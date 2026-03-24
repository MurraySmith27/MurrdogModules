/*
	Copyright © Carl Emil Carlsen 2021-2024
	http://cec.dk
*/

using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Simplex.Procedures
{
	public class ScalarTextureToSdfTextureProcedure
	{
		RenderTexture _sdfTexture;
		RenderTexture _floodTexture;

		ComputeShader _computeShader;
		int _SeedKernel;
		int _FloodKernel;
		int _DistKernel;
		int _ShowSeedsKernel;
		int _FillFromPrevKernel;

		LocalKeyword _ADD_BORDERS;

		Vector2Int _resolution;

		const int threadGroupWidth = 8; // Must match define in compute shader.

		public RenderTexture sdfTexture => _sdfTexture;

		[System.Serializable] public enum DownSampling { None, Half, Quater }
		[System.Serializable] public enum Precision { _16, _32 }


		static class ShaderIDs
		{
			public static readonly int _SeedTexRead = Shader.PropertyToID( nameof( _SeedTexRead ) );
			public static readonly int _FloodTex = Shader.PropertyToID( nameof( _FloodTex ) );
			public static readonly int _FloodTexRead = Shader.PropertyToID( nameof( _FloodTexRead ) );
			public static readonly int _SdfTex = Shader.PropertyToID( nameof( _SdfTex ) );
			public static readonly int _Resolution = Shader.PropertyToID( nameof( _Resolution ) );
			public static readonly int _StepSize = Shader.PropertyToID( nameof( _StepSize ) );
			public static readonly int _SeedThreshold = Shader.PropertyToID( nameof( _SeedThreshold ) );
			public static readonly int _TexelSize = Shader.PropertyToID( nameof( _TexelSize ) );
			public static readonly int _RegionOffset = Shader.PropertyToID( nameof( _RegionOffset ) );
			public static readonly int _RegionSize = Shader.PropertyToID( nameof( _RegionSize ) );
			public static readonly int _PrevSdfTex = Shader.PropertyToID( nameof( _PrevSdfTex ) );
		}


		public ScalarTextureToSdfTextureProcedure()
		{
			_computeShader = Object.Instantiate( Resources.Load<ComputeShader>( nameof( ScalarTextureToSdfTextureProcedure ) ) );
			_computeShader.hideFlags = HideFlags.HideAndDontSave;

			_SeedKernel = _computeShader.FindKernel( nameof( _SeedKernel ) );
			_FloodKernel = _computeShader.FindKernel( nameof( _FloodKernel ) );
			_DistKernel = _computeShader.FindKernel( nameof( _DistKernel ) );
			_ShowSeedsKernel = _computeShader.FindKernel( nameof( _ShowSeedsKernel ) );
			_FillFromPrevKernel = _computeShader.FindKernel( nameof( _FillFromPrevKernel ) );

			_ADD_BORDERS = new LocalKeyword( _computeShader, nameof( _ADD_BORDERS ) );
		}


		/// <summary>
		/// Updates the full SDF texture from the source texture.
		/// </summary>
		public Texture Update
		(
			Texture sourceTexture, float sourceValueThreshold,
			DownSampling downSampling = DownSampling.None, Precision precision = Precision._32, bool addBorders = false,
			bool _showSource = false
		){
			return Update( sourceTexture, sourceValueThreshold, downSampling, precision, addBorders, _showSource, Vector2Int.zero, Vector2Int.zero );
		}


		/// <summary>
		/// Updates only a rectangular region of the SDF texture, leaving the rest intact.
		/// Seeds outside the region are preserved from previous updates and will correctly influence the region via flood propagation.
		/// If prevSdfTexture is provided, pixels outside the region are filled from it instead of left as zero.
		/// </summary>
		public Texture Update
		(
			Texture sourceTexture, float sourceValueThreshold,
			DownSampling downSampling, Precision precision, bool addBorders, bool _showSource,
			Vector2Int regionOffset, Vector2Int regionSize, Texture prevSdfTexture = null
		){
			if( !sourceTexture ) return _sdfTexture;

			// Ensure and adapt resources.
			Vector2Int resolution = new Vector2Int( sourceTexture.width, sourceTexture.height );
			switch( downSampling ) {
				case DownSampling.Half: resolution /= 2; break;
				case DownSampling.Quater: resolution /= 4; break;
			}
			GraphicsFormat sdfFormat;
			switch( precision ) {
				case Precision._16: sdfFormat = GraphicsFormat.R16_SFloat; break;
				default: sdfFormat = GraphicsFormat.R32_SFloat; break;
			}
			if( !_sdfTexture || _sdfTexture.width != resolution.x || _sdfTexture.height != resolution.y || _sdfTexture.graphicsFormat != sdfFormat ) {
				_sdfTexture?.Release();
				_sdfTexture = CreateTexture( "SdfTexture", resolution, sdfFormat );
				_computeShader.SetTexture( _ShowSeedsKernel, ShaderIDs._SdfTex, _sdfTexture );
				_computeShader.SetTexture( _DistKernel, ShaderIDs._SdfTex, _sdfTexture );
				_computeShader.SetTexture( _FillFromPrevKernel, ShaderIDs._SdfTex, _sdfTexture );
			}
			if( !_floodTexture || _floodTexture.width != resolution.x || _floodTexture.height != resolution.y ) {
				_floodTexture?.Release();
				_floodTexture = CreateTexture( "FloodTexture", resolution, GraphicsFormat.R32G32B32A32_UInt );
				_computeShader.SetTexture( _SeedKernel, ShaderIDs._FloodTex, _floodTexture );
				_computeShader.SetTexture( _FloodKernel, ShaderIDs._FloodTex, _floodTexture );
				_computeShader.SetTexture( _DistKernel, ShaderIDs._FloodTexRead, _floodTexture );
				_computeShader.SetTexture( _ShowSeedsKernel, ShaderIDs._FloodTexRead, _floodTexture );
				_computeShader.SetInts( ShaderIDs._Resolution, new int[]{ resolution.x, resolution.y } );
				_computeShader.SetVector( ShaderIDs._TexelSize, _sdfTexture.texelSize );
			}
			_resolution = resolution;

			// Determine effective region (zero regionSize means full texture).
			bool isPartial = regionSize != Vector2Int.zero;
			Vector2Int effectiveOffset = isPartial ? regionOffset : Vector2Int.zero;
			Vector2Int effectiveSize   = isPartial ? regionSize   : resolution;

			// Scale region to downsampled texture space.
			switch( downSampling ) {
				case DownSampling.Half:
					effectiveOffset = new Vector2Int( effectiveOffset.x / 2, effectiveOffset.y / 2 );
					effectiveSize   = new Vector2Int( Mathf.Max( 1, effectiveSize.x / 2 ), Mathf.Max( 1, effectiveSize.y / 2 ) );
					break;
				case DownSampling.Quater:
					effectiveOffset = new Vector2Int( effectiveOffset.x / 4, effectiveOffset.y / 4 );
					effectiveSize   = new Vector2Int( Mathf.Max( 1, effectiveSize.x / 4 ), Mathf.Max( 1, effectiveSize.y / 4 ) );
					break;
			}

			// Clamp to texture bounds.
			effectiveOffset = new Vector2Int(
				Mathf.Clamp( effectiveOffset.x, 0, resolution.x - 1 ),
				Mathf.Clamp( effectiveOffset.y, 0, resolution.y - 1 )
			);
			effectiveSize = new Vector2Int(
				Mathf.Clamp( effectiveSize.x, 1, resolution.x - effectiveOffset.x ),
				Mathf.Clamp( effectiveSize.y, 1, resolution.y - effectiveOffset.y )
			);

			// Set region uniforms.
			_computeShader.SetInts( ShaderIDs._RegionOffset, new int[]{ effectiveOffset.x, effectiveOffset.y } );
			_computeShader.SetInts( ShaderIDs._RegionSize,   new int[]{ effectiveSize.x,   effectiveSize.y   } );

			// Thread group counts for seed/dist kernels (THREAD_GROUP_WIDTH x THREAD_GROUP_WIDTH threads per group).
			Vector2Int groupCount = new Vector2Int(
				Mathf.CeilToInt( effectiveSize.x / (float) threadGroupWidth ),
				Mathf.CeilToInt( effectiveSize.y / (float) threadGroupWidth )
			);

			// Set keywords.
			if( _computeShader.IsKeywordEnabled( _ADD_BORDERS ) != addBorders ) _computeShader.SetKeyword( _ADD_BORDERS, addBorders );

			// Seed.
			_computeShader.SetTexture( _SeedKernel, ShaderIDs._SeedTexRead, sourceTexture );
			_computeShader.SetFloat( ShaderIDs._SeedThreshold, sourceValueThreshold );
			_computeShader.Dispatch( _SeedKernel, groupCount.x, groupCount.y, 1 );

			// Show seeds.
			if( _showSource ) {
				_computeShader.Dispatch( _ShowSeedsKernel, groupCount.x, groupCount.y, 1 );
				return _sdfTexture;
			}

			// Flood. Use full texture size for step count so seeds anywhere in the texture can reach the region.
			// Each flood step dispatches one group per pixel in the region (FloodKernel is [1,1,9] threads).
			int sizeMax = Mathf.Max( resolution.x, resolution.y );
			int stepMax = (int) Mathf.Log( Mathf.NextPowerOfTwo( sizeMax ), 2 );
			for( int n = stepMax; n >= 0; n-- ) {
				int stepSize = n > 0 ? (int) Mathf.Pow( 2, n ) : 1;
				_computeShader.SetInt( ShaderIDs._StepSize, stepSize );
				_computeShader.Dispatch( _FloodKernel, effectiveSize.x, effectiveSize.y, 1 );
			}

			// Compute SDF.
			_computeShader.Dispatch( _DistKernel, groupCount.x, groupCount.y, 1 );

			// Fill pixels outside the updated region from the previous SDF so they aren't left black.
			// if( isPartial && prevSdfTexture != null ) {
			// 	_computeShader.SetTexture( _FillFromPrevKernel, ShaderIDs._PrevSdfTex, prevSdfTexture );
			// 	Vector2Int fullGroupCount = new Vector2Int(
			// 		Mathf.CeilToInt( resolution.x / (float) threadGroupWidth ),
			// 		Mathf.CeilToInt( resolution.y / (float) threadGroupWidth )
			// 	);
			// 	_computeShader.Dispatch( _FillFromPrevKernel, fullGroupCount.x, fullGroupCount.y, 1 );
			// }

			return _sdfTexture;
		}


		public void Release()
		{
			_sdfTexture?.Release();
			_floodTexture?.Release();
			_sdfTexture = null;
			_floodTexture = null;
		}


		static RenderTexture CreateTexture( string name, Vector2Int resolution, GraphicsFormat format )
		{
			RenderTexture rt = new RenderTexture( resolution.x, resolution.y, 0, format, 0 );
			rt.name = name;
			rt.autoGenerateMips = false;
			rt.enableRandomWrite = true;
			rt.Create();
			return rt;
		}
	}
}
