using UnityEngine;

public class MaleUnitAppearance : MonoBehaviour
{
    static readonly string[] JobRootNames =
    {
        "M_Knight",
        "M_Swordsman",
        "M_Archer",
        "M_Mage",
        "M_Fighter",
        "M_Rogue",
        "M_Warlock",
        "M_Hunter",
        "M_Sorcerer"
    };

    public bool ApplyJob(UnitJob job)
    {
        string selectedRootName = GetJobRootName(job);
        if (string.IsNullOrEmpty(selectedRootName))
        {
            Debug.LogError($"[MaleUnitAppearance] 지원하지 않는 직업입니다: {job}", this);
            return false;
        }

        bool foundSelectedRoot = false;
        Transform[] children = GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (!IsJobRoot(child.name)) continue;

            bool shouldEnable = child.name == selectedRootName;
            child.gameObject.SetActive(shouldEnable);
            foundSelectedRoot |= shouldEnable;
        }

        if (!foundSelectedRoot)
        {
            Debug.LogError($"[MaleUnitAppearance] '{selectedRootName}' 오브젝트를 찾지 못했습니다.", this);
        }

        return foundSelectedRoot;
    }

    static bool IsJobRoot(string objectName)
    {
        foreach (string jobRootName in JobRootNames)
        {
            if (objectName == jobRootName) return true;
        }

        return false;
    }

    static string GetJobRootName(UnitJob job)
    {
        return job switch
        {
            UnitJob.Knight => "M_Knight",
            UnitJob.Swordsman => "M_Swordsman",
            UnitJob.Archer => "M_Archer",
            UnitJob.Mage => "M_Mage",
            UnitJob.Fighter => "M_Fighter",
            UnitJob.Rogue => "M_Rogue",
            UnitJob.Warlock => "M_Warlock",
            UnitJob.Hunter => "M_Hunter",
            UnitJob.Sorcerer => "M_Sorcerer",
            _ => null
        };
    }
}
