using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.InteractableBehaviour;
using UnityEngine;
using System.Collections;

public class CarryElements : MonoBehaviour {
    internal List<Transform> carryList;
    public float fadeCarry;
    public Transform TBouquet;
    private bool firstAfter = true;
    public Transform FlowerHead;
    public Transform Branch;
    private int currentRot;
    private const int ADD_ROT = 135;
    Transform[] branchChildren;

	// Use this for initialization
    void Awake()
    {
        carryList = new List<Transform>();

        branchChildren = Branch.GetComponentsInChildren<Transform>();
        foreach (Transform child in branchChildren) {
            child.transform.gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void UpdateCarry(float progress)
    {
        if (progress < FlowerBehaviour.RealFlowerPick)
        {
            FadeCarryUpdate();
            firstAfter = true;
        }
        else if (firstAfter)
        {
            List<Transform> flowers = carryList.ToList();
            flowers.ForEach(e => ChangeTransparancy(0.5f, e));
            firstAfter = false;
        }
    }

    private void FadeCarryUpdate()
    {
        if (fadeCarry > 0)
        {
            List<Transform> flowers = carryList.ToList();
            flowers.ForEach(e => ChangeTransparancy(fadeCarry, e));
            fadeCarry -= Time.deltaTime;
        }

        if (fadeCarry <= 0)
        {
            fadeCarry = 0;
            ClearFlowers();
        }
    }

    private void ChangeTransparancy(float fadeRemain, Transform carryObject)
    {
        Color extColor = carryObject.GetComponent<Renderer>().material.color;
        carryObject.GetComponent<Renderer>().material.color = new Color(extColor.r, extColor.g, extColor.b, (fadeRemain / 5));
    }

    private float GetNewFadeCarryDuration(float progress)
    {
        //0.0 => 2 sec
        //0.3 => 5 sec
        return 2 + (progress * 10);
    }

    public void PickFlower(float progress)
    {
        if (progress <= FlowerBehaviour.RealFlowerPick)
        {
            fadeCarry = GetNewFadeCarryDuration(progress);
        }

        Quaternion rota = new Quaternion();
        Transform flower = Instantiate(FlowerHead, transform.position, rota) as Transform;
        Debug.Assert(flower != null, "flower != null");
        flower.parent = transform;

        carryList.Add(flower);
    }

    public void PickBranch(float progress) {
        foreach (Transform child in branchChildren) {
            child.transform.gameObject.SetActive(true);
        }
    }

    public void EatBerry(float progress)
    {
        //ThrowBouquet();
    }

    private float GetNextAngle()
    {
        currentRot += ADD_ROT;
        return currentRot;
    }

    private void ClearFlowers()
    {
        Transform[] arr = carryList.ToArray();
        for (int i = 0; i < arr.Length; i++)
        {
            Destroy(arr[i].gameObject);
        }
        carryList.Clear();
        currentRot = 0;
    }

    public void ThrowBouquet()
    {
        if (carryList.Any())
        {
            ClearFlowers();
            Instantiate(TBouquet, transform.position, new Quaternion());
        }
        
    }
}
