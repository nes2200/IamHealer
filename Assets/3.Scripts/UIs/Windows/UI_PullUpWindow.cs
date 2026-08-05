using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public class UI_PullUpWindow : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }


    public int[] solution(int n, int m)
    {
        int[] answer = new int[2];

        int a = n, b = m;
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        answer[0] = a;

        a = n; b = m;
        answer[1] = a * b / answer[0];
        return answer;
    }
}
