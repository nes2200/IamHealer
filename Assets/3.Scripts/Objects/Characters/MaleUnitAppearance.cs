using UnityEngine;

public class MaleUnitAppearance : MonoBehaviour
{
    //장식물
    static readonly string[] KnightDecorationNames =
    {
        "GreatHelm", "Pauldrons", "NeckScarf", "SwordSheathed"
    }; 
    static readonly string[] SwordsmanDecorationNames =
    {
        "SkullCap"
    };

    //갑옷 색
    const string ObjectsMaterialName = "RGBRecolor_Objects";
    static readonly int Color1Id = Shader.PropertyToID("_Color1");
    static readonly int Color2Id = Shader.PropertyToID("_Color2");
    static readonly int Color3Id = Shader.PropertyToID("_Color3");
    Material objectsMaterialInstance;

    public bool ApplyAppearance(UnitJob job)
    {
        return job switch
        {
            UnitJob.Knight => ApplyKnightAppearance(),
            UnitJob.Swordsman => ApplySwordsmanAppearance(),
            _ => true
        };
    }

    bool ApplyKnightAppearance()
    {
        Transform knightRoot = FindChildByName(transform, "M_Knight");

        if(!knightRoot)
        {
            Debug.LogError( "[MaleUnitAppearance] M_Knight를 찾지 못했습니다.", this);
            return false;
        }
        RandomizeDecorations(knightRoot, KnightDecorationNames);

        Transform greatHelm = FindChildByName(knightRoot, "M_Knight_GreatHelm");

        if (!greatHelm)
        {
            Debug.LogError("[MaleUnitAppearance] GreatHelm을 찾지 못했습니다.", this);
            return false;
        }

        bool helmEnabled = greatHelm.gameObject.activeSelf;

        return ApplyHeadAppearance(faceCovered: helmEnabled, hairCovered: helmEnabled);
    }
    bool ApplySwordsmanAppearance()
    {
        Transform swordsmantRoot = FindChildByName(transform, "M_Swordsman");

        if (!swordsmantRoot)
        {
            Debug.LogError("[MaleUnitAppearance] M_Swordsman을 찾지 못했습니다.", this);
            return false;
        }

        RandomizeDecorations(swordsmantRoot, SwordsmanDecorationNames);

        Transform skullcap = FindChildByName(swordsmantRoot, "M_Swordsman_SkullCap");
        

        if (!skullcap)
        {
            Debug.LogError("[MaleUnitAppearance] SkullCap을 찾지 못했습니다.", this);
            return false;
        }

        return ApplyHeadAppearance(faceCovered: false, hairCovered: skullcap.gameObject.activeSelf);
    }

    bool ApplyHeadAppearance(bool faceCovered, bool hairCovered)
    {
        Transform face = FindChildByName(transform, "Face");
        Transform facialHair = FindChildByName(transform, "FacialHair");
        Transform hair = FindChildByName(transform, "Hair");
        Transform hairForHeadwear = FindChildByName(transform, "Hair_forHeadwear");

        if (!face || !facialHair || !hair)
        {
            Debug.LogError("[MaleUnitAppearance] 공통 머리 외형 오브젝트를 찾지 못했습니다.", this);
            return false;
        }

        // 현재는 사용하지 않음
        if (hairForHeadwear)
        {
            hairForHeadwear.gameObject.SetActive(false);
        }

        // GreatHelm처럼 얼굴 전체를 가리는 경우
        if (faceCovered)
        {
            face.gameObject.SetActive(false);
            facialHair.gameObject.SetActive(false);
            hair.gameObject.SetActive(false);

            return true;
        }

        face.gameObject.SetActive(true);
        facialHair.gameObject.SetActive(true);

        Transform eyebrows = FindChildByName(face, "Eyebrows");
        Transform eyes = FindChildByName(face, "Eyes");
        Transform mouths = FindChildByName(face, "Mouths");

        if (!eyebrows || !eyes || !mouths)
        {
            Debug.LogError("[MaleUnitAppearance] Face 아래의 Eyebrows, Eyes, Mouths를 찾지 못했습니다.", this);
            return false;
        }

        SetOnlyRandomChildActive(eyebrows, false);
        SetOnlyRandomChildActive(eyes, false);
        SetOnlyRandomChildActive(mouths, false);
        RandomizeEachChild(facialHair);

        if (hairCovered)
        {
            hair.gameObject.SetActive(false);
        }
        else
        {
            hair.gameObject.SetActive(true);
            SetOnlyRandomChildActive(hair, true);
        }

        return true;
    }

    bool CreateRandomObjectsMaterial(Transform jobRoot)
    {
        if (objectsMaterialInstance) return true;

        Renderer[] renderers = jobRoot.GetComponentsInChildren<Renderer>(true);

        foreach(Renderer targetRenderer in renderers)
        {
            Material[] materials = targetRenderer.sharedMaterials;
            bool materialsChanged = false;

            for(int i = 0; i < materials.Length; i++)
            {
                Material sourceMaterial = materials[i];

                if (!sourceMaterial || sourceMaterial.name != ObjectsMaterialName) continue;
                //유닛 전용 마테리얼은 최초 한번만 생성
                if (!objectsMaterialInstance)
                {
                    objectsMaterialInstance = new Material(sourceMaterial);
                    objectsMaterialInstance.name = $"{ObjectsMaterialName}_Runtime";
                    objectsMaterialInstance.SetColor(Color1Id, CreateRandomColor());
                    objectsMaterialInstance.SetColor(Color2Id, CreateRandomColor());
                    objectsMaterialInstance.SetColor(Color3Id, CreateRandomColor());

                }
                //모든 Rendere에 동일한 인스턴스 연결
                materials[i] = objectsMaterialInstance;
                materialsChanged = true;
            }
            //반복문 후 한번만 적용
            if (materialsChanged) targetRenderer.sharedMaterials = materials;
        }

        if (!objectsMaterialInstance)
        {
            Debug.LogError($"[MaleUnitAppearance] '{ObjectsMaterialName}'을 찾지 못했습니다.", this);
            return false;
        }
        return true;
    }

    static Color CreateRandomColor()
    {
        return Random.ColorHSV(0f, 1f, 0.35f, 1f, 0.35f, 1f);
    }


    static void SetOnlyRandomChildActive(Transform group, bool allowNone)
    {
        int selectedIndex = allowNone
            ? Random.Range(-1, group.childCount)
            : Random.Range(0, group.childCount);

        for (int i = 0; i < group.childCount; i++)
        {
            group.GetChild(i).gameObject.SetActive(i == selectedIndex);
        }
    }

    static void RandomizeEachChild(Transform group)
    {
        for (int i = 0; i < group.childCount; i++)
        {
            group.GetChild(i).gameObject.SetActive(Random.value < 0.5f);
        }
    }

    static void RandomizeDecorations(Transform root, string[] decorationNames)
    {
        Transform[] children = root.GetComponentsInChildren<Transform>(true);

        foreach(string decorationName in decorationNames)
        {
            string targetName = $"{root.name}_{decorationName}";

            Transform decoration = null;

            foreach(Transform child in children)
            {
                if (child.name != targetName) continue;

                decoration = child;
                break;
            }

            if (!decoration)
            {
                Debug.LogWarning($"[MaleUnitAppearance] '{targetName}'을 찾지 못했습니다.");
                continue;
            }

            decoration.gameObject.SetActive(Random.value < 0.5f);
        }
    }

    static Transform FindChildByName(Transform root, string childName)
    {
        Transform[] children = root.GetComponentsInChildren<Transform>(true);

        foreach(Transform child in children)
        {
            if(child.name == childName)
            {
                return child;
            }
        }
        return null;
    }

    void OnDestroy()
    {
        if (objectsMaterialInstance)
        {
            Destroy(objectsMaterialInstance);
        }
    }
}
