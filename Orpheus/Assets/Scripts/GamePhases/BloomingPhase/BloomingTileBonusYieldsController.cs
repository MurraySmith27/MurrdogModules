using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;

public class BloomingTileBonusYieldsController : Singleton<BloomingTileBonusYieldsController>
{

    [SerializeField] private float startWaitTime = 0.2f;
    [SerializeField] private float endWaitTime = 0.2f;
    [SerializeField] private float cityStartAnimationTime = 0.5f;
    [SerializeField] private float cityEndAnimationTime = 0.5f;
    [SerializeField] private float tileBonusYieldSourceStartTime = 0.25f;
    [SerializeField] private float tileBonusYieldSourceEndTime = 0.25f;
    [SerializeField] private float tileAnimationEndTime = 0.3f;
    [SerializeField] private float tileAnimationPerYieldBonus = 0.1f;
    [SerializeField] private float tileAnimationRelicBonus = 0.1f;

    public event Action OnBonusYieldsStart;
    public event Action OnBonusYieldsEnd;
    public event Action<Guid> OnCityTileBonusYieldsStart;
    public event Action<Guid> OnCityTileBonusYieldsEnd;

    public event Action<Vector2Int> OnTileYieldBonusSourceStart;
    public event Action<Vector2Int> OnTileYieldBonusSourceEnd;
    
    //source tile, destination tile, yield diff
    public event Action<Vector2Int, Vector2Int, int> OnTileYieldBonusGranted;
    
    //relic type, source tile, destination tile, yield diff
    public event Action<RelicTypes, Vector2Int, Vector2Int, int> OnRelicTriggered;
    
    public void StartTileBonusYields()
    {
        Timing.RunCoroutineSingleton(TileBonusYieldsCoroutine(), this.gameObject, SingletonBehavior.Wait);
    }

    private IEnumerator<float> TileBonusYieldsCoroutine()
    {
        OnBonusYieldsStart?.Invoke();
        
        yield return OrpheusTiming.WaitForSecondsGameTime(startWaitTime);

        Dictionary<Vector2Int, int> currentYieldBonus = new();
        
        List<Guid> cityGuids = MapSystem.Instance.GetAllCityGuids();

        foreach (Guid cityGuid in cityGuids)
        {
            OnCityTileBonusYieldsStart?.Invoke(cityGuid);
            
            yield return OrpheusTiming.WaitForSecondsGameTime(cityStartAnimationTime);
            
            List<(Vector2Int, Vector2Int, int)> allTileYieldBonuses;
            if (TerrainBonusSystem.Instance.GetTileYieldBonuses(out allTileYieldBonuses))
            {
                Dictionary<Vector2Int, List<(Vector2Int, int)>> tileBonusesSortedBySource = new();

                foreach (var bonus in allTileYieldBonuses)
                {
                    if (!tileBonusesSortedBySource.ContainsKey(bonus.Item1))
                    {
                        tileBonusesSortedBySource.Add(bonus.Item1, new List<(Vector2Int, int)>());   
                    }
                    
                    tileBonusesSortedBySource[bonus.Item1].Add((bonus.Item2, bonus.Item3));
                }

                foreach (Vector2Int source in tileBonusesSortedBySource.Keys)
                {
                    OnTileYieldBonusSourceStart?.Invoke(source);

                    yield return OrpheusTiming.WaitForSecondsGameTime(tileBonusYieldSourceStartTime);

                    foreach (var pair in tileBonusesSortedBySource[source])
                    {
                        int yieldDifference;
                        
                        List<(RelicTypes, int)> triggeredRelics = RelicSystem.Instance.OnTileYieldBonusesApplied(source, pair.Item1, pair.Item2, out yieldDifference);

                        foreach (var relicPair in triggeredRelics)
                        {
                            //relic type, source, dest, yield diff
                            OnRelicTriggered?.Invoke(relicPair.Item1, source, pair.Item1, relicPair.Item2);
                            
                            yield return OrpheusTiming.WaitForSecondsGameTime(tileAnimationRelicBonus);
                        }
                        
                        if (!currentYieldBonus.ContainsKey(pair.Item1))
                        {
                            currentYieldBonus.Add(pair.Item1, 0);
                        }

                        currentYieldBonus[pair.Item1] += yieldDifference;
                        
                        OnTileYieldBonusGranted?.Invoke(source, pair.Item1, pair.Item2);

                        yield return OrpheusTiming.WaitForSecondsGameTime(tileAnimationPerYieldBonus);
                    }

                    OnTileYieldBonusSourceEnd?.Invoke(source);

                    yield return OrpheusTiming.WaitForSecondsGameTime(tileBonusYieldSourceEndTime);
                }
            }
            
            OnCityTileBonusYieldsEnd?.Invoke(cityGuid);
            
            yield return Timing.WaitForSeconds(cityEndAnimationTime);
        }
        
        HarvestState.Instance.RegisterCurrentRoundYieldBonuses(currentYieldBonus);
        
        yield return OrpheusTiming.WaitForSecondsGameTime(endWaitTime);
        
        OnBonusYieldsEnd?.Invoke();
    }
}
