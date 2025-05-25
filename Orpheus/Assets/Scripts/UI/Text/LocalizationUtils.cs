using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalizationUtils
{


    public static string GetNameOfResource(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Wood:
                return "Wood";
            case ResourceType.Stone:
                return "Stone";
            case ResourceType.Corn:
                return "Corn";
            case ResourceType.Wheat:
                return "Wheat";
            case ResourceType.Fish:
                return "Fish";
            case ResourceType.Bread:
                return "Bread";
            case ResourceType.Lumber:
                return "Lumber";
            case ResourceType.Copper:
                return "Copper";
            case ResourceType.Steel:
                return "Steel";
            case ResourceType.Popcorn:
                return "Popcorn";
            case ResourceType.Sushi:
                return "Sushi";
            case ResourceType.Flour:
                return "Flour";
            case ResourceType.Dough:
                return "Dough";
            case ResourceType.Toast:
                return "Toast";
            case ResourceType.ButteredToast:
                return "ButteredToast";
            case ResourceType.Butter:
                return "Butter";
            case ResourceType.Milk:
                return "Milk";
            default:
                return "";
        }
    }

    public static string GetNameOfBuilding(BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.CityCapital:
                return "Castle";
            case BuildingType.CornFarm:
                return "Corn Farm";
            case BuildingType.WheatFarm:
                return "Wheat Farm";
            case BuildingType.Bakery:
                return "Bakery";
            case BuildingType.FishFarm:
                return "Fish Farm";
            case BuildingType.LumberMill:
                return "Lumber Mill";
            case BuildingType.CopperYard:
                return "Copper Yard";
            case BuildingType.SteelYard:
                return "Steel Factory";
            case BuildingType.PopcornFactory:
                return "Popcorn Factory";
            case BuildingType.SushiRestaurant:
                return "Sushi Restaurant";
            case BuildingType.Forest:
                return "Forest";
            case BuildingType.Mine:
                return "Mine";
            case BuildingType.Well:
                return "Well";
            case BuildingType.DirtPile:
                return "Dirt Pile";
            case BuildingType.Mill:
                return "Mill";
            case BuildingType.Toaster:
                return "Toaster";
            case BuildingType.CowFarm:
                return "Cow Farm";
            case BuildingType.Stirrer:
                return "Stirrer";
            case BuildingType.Mixer:
                return "Cutting Board";
            default:
                return "";
        }
    }

    public static string GetTagForPersistentResource(PersistentResourceType persistentResourceType)
    {
        switch (persistentResourceType)
        {
            case PersistentResourceType.Wood:
                return "<wood>";
            case PersistentResourceType.Stone:
                return "<stone>";
            case PersistentResourceType.Gold:
                return "<gold>";
            case PersistentResourceType.BuildToken:
                return "<buildtoken>";
            case PersistentResourceType.Water:
                return "<water>";
            case PersistentResourceType.Dirt:
                return "<dirt>";
            case PersistentResourceType.Oil:
                return "<oil>";
            default:
                return "";
        }
    }
    
    public static string GetIconTagForPersistentResource(PersistentResourceType persistentResourceType)
    {
        switch (persistentResourceType)
        {
            case PersistentResourceType.Wood:
                return "<sprite index=1>";
            case PersistentResourceType.Stone:
                return "<sprite index=7>";
            case PersistentResourceType.Gold:
                return "<sprite index=0>";
            case PersistentResourceType.BuildToken:
                return "<sprite index=13>";
            case PersistentResourceType.Water:
                return "<sprite index=4>";
            case PersistentResourceType.Dirt:
                return "<sprite index=5>";
            case PersistentResourceType.Oil:
                return "<sprite index=17>";
            default:
                return "";
        }
    }
    
    public static string GetIconTagForResource(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Corn:
                return "<sprite index=6>";
                break;
            case ResourceType.Wheat:
                return "<sprite index=14>";
                break;
            case ResourceType.Fish:
                return "<sprite index=10>";
                break;
            case ResourceType.Wood:
                return "<sprite index=1>";
                break;
            case ResourceType.Stone:
                return "<sprite index=7>";
                break;
            case ResourceType.Bread:
                return "<sprite index=15>";
                break;
            case ResourceType.Lumber:
                return "<sprite index=12>";
                break;
            case ResourceType.Copper:
                return "<sprite index=8>";
                break;
            case ResourceType.Steel:
                return "<sprite index=16>";
                break;
            case ResourceType.Popcorn:
                return "<sprite index=9>";
                break;
            case ResourceType.Sushi:
                return "<sprite index=3>";
                break;
            case ResourceType.Flour:
                return "<sprite index=20>";
            case ResourceType.Dough:
                return "<sprite index=18>";
            case ResourceType.Toast:
                return "<sprite index=21>";
            case ResourceType.ButteredToast:
                return "<sprite index=23>";
            case ResourceType.Butter:
                return "<sprite index=19>";
            case ResourceType.Milk:
                return "<sprite index=22>";
            default:
                return "";
        }
    }
    
    public static string GetTagForResource(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Wood:
                return "<wood>";
            case ResourceType.Stone:
                return "<stone>";
            case ResourceType.Corn:
                return "<corn>";
            case ResourceType.Wheat:
                return "<wheat>";
            case ResourceType.Fish:
                return "<fish>";
            case ResourceType.Bread:
                return "<bread>";
            case ResourceType.Lumber:
                return "<lumber>";
            case ResourceType.Copper:
                return "<copper>";
            case ResourceType.Steel:
                return "<steel>";
            case ResourceType.Popcorn:
                return "<popcorn>";
            case ResourceType.Sushi:
                return "<sushi>";
            case ResourceType.Flour:
                return "<flour>";
            case ResourceType.Dough:
                return "<dough>";
            case ResourceType.Toast:
                return "<toast>";
            case ResourceType.ButteredToast:
                return "<butteredtoast>";
            case ResourceType.Butter:
                return "<butter>";
            case ResourceType.Milk:
                return "<milk>";
            default:
                return "";
        }
    }

    public static string GetTagForBuilding(BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.CityCapital:
                return "<citycapital>";
            case BuildingType.CornFarm:
                return "<cornfarm>";
            case BuildingType.FishFarm:
                return "<fishfarm>";
            case BuildingType.WheatFarm:
                return "<wheatfarm>";
            case BuildingType.Bakery:
                return "<bakery>";
            case BuildingType.LumberMill:
                return "<lumbermill>";
            case BuildingType.CopperYard:
                return "<copperyard>";
            case BuildingType.SteelYard:
                return "<steelyard>";
            case BuildingType.PopcornFactory:
                return "<popcornfactory>";
            case BuildingType.SushiRestaurant:
                return "<sushirestaurant>";
            case BuildingType.Forest:
                return "<forest>";
            case BuildingType.Mine:
                return "<mine>";
            case BuildingType.Well:
                return "<well>";
            case BuildingType.DirtPile:
                return "<dirtpile>";
            case BuildingType.Mill:
                return "<mill>";
            case BuildingType.Toaster:
                return "<toaster>";
            case BuildingType.CowFarm:
                return "<cowfarm>";
            case BuildingType.Stirrer:
                return "<stirrer>";
            case BuildingType.Mixer:
                return "<mixer>";
            default:
                return "";
        }
    }

    public static string GetDescriptionOfBuilding(BuildingType buildingType, BuildingsVisualsSO buildingVisualsSO, BuildingProcessRulesSO buildingProcessRulesSO, bool includeTitle = true)
    {
        string basicDescription = buildingVisualsSO.GetDescriptionForBuilding(buildingType);

        string additionalDescription = string.IsNullOrEmpty(basicDescription) ? "" : "\n";

        int numLanes = buildingProcessRulesSO.GetNumProcessLanes(buildingType);

        bool first = true;
        for (int i = 0; i < numLanes; i++)
        {
            if (!first)
            {
                additionalDescription += "\n";
            }
                    
            first = false;
            
            var input = buildingProcessRulesSO.GetResourceInput(buildingType, i);

            var output = buildingProcessRulesSO.GetResourceOutput(buildingType, i);

            if (input.Count > 0)
            {
                foreach (ResourceItem resourceItem in input)
                {
                    additionalDescription += LocalizationUtils.GetIconTagForResource(resourceItem.Type) +
                                             (resourceItem.Quantity > 1 ? resourceItem.Quantity.ToString() : "");
                }

                additionalDescription += " > ";
            }
            else if (output.Count > 0)
            {
                additionalDescription += "Harvests ";
            }
            

            foreach (ResourceItem resourceItem in output)
            {
                additionalDescription += LocalizationUtils.GetIconTagForResource(resourceItem.Type) +
                                         (resourceItem.Quantity > 1 ? resourceItem.Quantity.ToString() : "");
            }
            
            var persistentOutput = buildingProcessRulesSO.GetPersistentResourceOutput(buildingType, i);

            if (persistentOutput.Count > 0)
            {
                additionalDescription += "\nGrants ";

                foreach (PersistentResourceItem persistentResourceItem in persistentOutput)
                {
                    additionalDescription +=
                        LocalizationUtils.GetIconTagForPersistentResource(persistentResourceItem.Type) +
                        persistentResourceItem.Quantity.ToString();
                }
            }
        }
        
        var persistentInput = buildingProcessRulesSO.GetPersistentResourceInput(buildingType, 0);

        if (persistentInput.Count > 0)
        {
            additionalDescription += "\nCosts ";

            foreach (PersistentResourceItem persistentResourceItem in persistentInput)
            {
                additionalDescription +=
                    LocalizationUtils.GetIconTagForPersistentResource(persistentResourceItem.Type) +
                    persistentResourceItem.Quantity.ToString();
            }
        }

        string description = basicDescription + additionalDescription;

        if (description.StartsWith("\n"))
        {
            description = description.Substring(1);
        }

        if (includeTitle)
        {
            description = $"<b>{LocalizationUtils.GetNameOfBuilding(buildingType)}</b>\n{description}";
        }

        return description;
    }
}
