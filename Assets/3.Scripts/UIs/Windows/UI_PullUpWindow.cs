using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;


public class UI_PullUpWindow : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public int[] solution(int[] array, int[,] commands)
    {
        int[] answer = new int[commands.GetLength(0)];
        for(int i = 0; i < answer.Length; i++)
        {
            //int[] temp = new();
            //array.CopyTo(temp, commands[i, 0], commands[i, 1]);
        }
        return answer;
    }

}
