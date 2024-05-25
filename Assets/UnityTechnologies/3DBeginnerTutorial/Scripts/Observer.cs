using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    //检查玩家的Transform而非游戏对象，可以更轻松地了解到位置并确定是否能清楚看到他
    public Transform player;
    public GameEnding gameEnding;//结束游戏引用GameEnding类

    bool m_IsPlayerInRange;//角色是否在触发器中

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player) //每次调用都检查玩家角色是否在范围中
        {
            m_IsPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)//判断离开触发器，与OnTriggerEnter相反
    {
        if (other.transform == player)//每次调用都检查玩家角色是否在范围中
        {
            m_IsPlayerInRange = false;
        }
    }

    void Update()
    {
        if (m_IsPlayerInRange) 
        {
            //方向是玩家位置减去视点位置，玩家位置在双脚之间的地面上，为了可以看到质心，添加一个向上的单位
            Vector3 direction = player.position - transform.position + Vector3.up;//Vector3.up是(0,1,0)的快捷方式
            Ray ray = new Ray(transform.position, direction);//创建射线
            RaycastHit raycastHit;//用于存储射线命中对象的信息

            if (Physics.Raycast(ray, out raycastHit)) 
            {
                if (raycastHit.collider.transform == player)
                {
                    gameEnding.CaughtPlayer();//访问公共方法传递被抓的信息
                }
            }
        }
    }
}
