using Coffee.UIMaterialPropertyInjectorInternal;
using UnityEngine;
using UnityEngine.Profiling;

namespace Coffee.UIExtensions
{
    [RequireComponent(typeof(Renderer))]
    [ExecuteAlways]
    [Icon("Packages/com.coffee.ui-material-property-injector/Icons/UIMaterialPropertyInjectorIcon.png")]
    public class RendererMaterialPropertyInjector : UIMaterialPropertyInjector
    {
        private static readonly InternalObjectPool<MaterialPropertyBlock> s_MpbPool =
            new InternalObjectPool<MaterialPropertyBlock>(() => new MaterialPropertyBlock(), _ => true, x => x.Clear());

        public new Renderer renderer => _renderer ? _renderer : _renderer = GetComponent<Renderer>();
        public override Material material => Application.isPlaying ? renderer.material : renderer.sharedMaterial;
        public override Material defaultMaterialForRendering => renderer.sharedMaterial;

        private Renderer _renderer;
        private MaterialPropertyBlock _mpb;

        protected override void OnEnable()
        {
            _mpb = s_MpbPool.Rent();
            base.OnEnable();
            InjectIfNeeded();
        }

        protected override void OnDisable()
        {
            if (renderer)
            {
                renderer.SetPropertyBlock(null);
            }

            s_MpbPool.Return(ref _mpb);
            base.OnDisable();
        }

        protected override void InjectIfNeeded()
        {
            // Skip if not dirty.
            if (!_dirty || !canInject || !renderer) return;
            _dirty = false;

            // Inject properties to materials.
            Profiler.BeginSample("(MPI)[Injector] InjectIfNeeded > Inject properties to materials");
            renderer.GetPropertyBlock(_mpb);
            for (var i = 0; i < properties.Count; i++)
            {
                properties[i].Inject(_mpb);
            }

            renderer.SetPropertyBlock(_mpb);
            Profiler.EndSample();
        }
    }
}
