using System.Collections.Generic;

//另写一个buildingInfo类是为了对于每种建筑都只读取一次配置文件
public class BuildingInfo
{
    public string name;
    public int type;
    public string restriction;
    public int extraRestrictionId;
    public string neighborBonus;
    public int sizeX;
    public int sizeY;
    public string massProduction;
    public string singleProduction;
    public int livabilityBoost;
    public int maxCount;
    public int maxLevel;
    public string upgradeCost;
    public string upgradeDuration;
    public float stationBonus;
    public string description;
    public string effect;
    public string upgradeRestriction;
    public int group;
    public string nameChinese;
}
public class BuildingInfoPro
{
    public BuildingInfo buildingInfo;
    public int restrictionMask;
    public Dictionary<string, float> neighborBonusDict;
    public List<Dictionary<string, float>> massProductionList;
    public List<Dictionary<string, float>> singleProductionList;
    public List<Dictionary<string, float>> costList;
    public List<float> durationList;
    public List<Dictionary<string, int>> upgradeRestrictionList;
    public BuildingInfoPro(BuildingInfo info)
    {
        buildingInfo = info;
        string[] parts = buildingInfo.restriction.Split(';');
        restrictionMask = 0;
        foreach (string part in parts)
        {
            restrictionMask |= 1 << (int.Parse(part) - 1);
        }
        neighborBonusDict = new Dictionary<string, float>();
        parts = buildingInfo.neighborBonus.Split(",");
        if (parts[0] != "")
            neighborBonusDict[parts[0]] = float.Parse(parts[1]);
        massProductionList = new List<Dictionary<string, float>>();
        parts = buildingInfo.massProduction.Split(";");
        foreach (string part in parts)
        {
            Dictionary<string, float> dic = new Dictionary<string, float>();
            string[] pairs = part.Split(",");
            foreach (string pair in pairs)
            {
                string[] element = pair.Split("|");
                if (element[0] != "")
                    dic[element[0]] = float.Parse(element[1]);
            }
            massProductionList.Add(dic);
        }
        singleProductionList = new List<Dictionary<string, float>>();
        parts = buildingInfo.singleProduction.Split(";");
        foreach (string part in parts)
        {
            Dictionary<string, float> dic = new Dictionary<string, float>();
            string[] pairs = part.Split(",");
            foreach (string pair in pairs)
            {
                string[] element = pair.Split("|");
                if (element[0] != "")
                    dic[element[0]] = float.Parse(element[1]);
            }
            singleProductionList.Add(dic);
        }
        costList = new List<Dictionary<string, float>>();
        parts = buildingInfo.upgradeCost.Split(";");
        foreach (string part in parts)
        {
            Dictionary<string, float> dic = new Dictionary<string, float>();
            string[] pairs = part.Split(",");
            foreach (string pair in pairs)
            {
                string[] element = pair.Split("|");
                if (element[0] != "")
                    dic[element[0]] = float.Parse(element[1]);
            }
            costList.Add(dic);
        }
        durationList = new List<float>();
        parts = buildingInfo.upgradeDuration.Split(";");
        foreach (string part in parts)
        {
            durationList.Add(float.Parse(part));
        }
        upgradeRestrictionList = new List<Dictionary<string, int>>();
        parts = buildingInfo.upgradeRestriction.Split(";");
        foreach (string part in parts)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            string[] pairs = part.Split(",");
            foreach (string pair in pairs)
            {
                string[] element = pair.Split("|");
                if (element[0] != "")
                    dic[element[0]] = int.Parse(element[1]);
            }
            upgradeRestrictionList.Add(dic);
        }
    }
}
//我之前写的那个XmlDataManager不这样封装个一层就用不了...
public class BuildingInfoCollection
{
    public List<BuildingInfo> buildingInfos;
}
