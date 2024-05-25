using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;//转速（以弧度为单位）（公共成员变量，不需要带"m_"前缀）
    
    Animator m_Animator;//将对该组件的引用保留为成员变量，因为该引用在整个类中使用
    Rigidbody m_Rigidbody;//和上面的引用一样，声明对Ridgidbody组件进行引用
    AudioSource m_AudioSource;//同上
    Vector3 m_Movement;//移动矢量
    Quaternion m_Rotation = Quaternion.identity;//四元数 (Quaternion) 是存储旋转的一种方式，可用于解决将旋转存储为 3D 矢量时遇到的一些问题。

    // Start is called before the first frame update
    void Start()
    {
        //获取对"Animator"类型组件的引用，并将其分配给变量m_Animator
        m_Animator = GetComponent<Animator>(); //(GetComponent是MonoBehaviour的一部分)
        m_Rigidbody = GetComponent<Rigidbody>();//设置Rigidbody变量的引用
        m_AudioSource = GetComponent<AudioSource>();//同上
    }

    // Update is called once per frame
    /* 已经确保 Animator 通过物理循环适时运行，从而避免物理与动画之间发生冲突。但是，现在将使用 OnAnimatorMove 来覆盖根运动。
     * 这意味着 OnAnimatorMove 实际上将通过物理适时被调用，而不是像 Update 方法那样通过渲染被调用。
     * 移动矢量和旋转在 Update 中加以设置。如果首先调用的是 OnAnimatorMove，则由于未设值的四元数没有任何意义，因此将遇到问题。
     * 这是 Unity 自动调用的另一个特殊方法，但是这个方法是通过物理适时调用的。
     * FixedUpdate 不是在每个渲染的帧之前被调用，而是在物理系统处理所有碰撞和其他已发生的交互之前被调用。
     * 默认情况下，每秒正好调用 50 次这个方法。*/
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");//水平轴
        float vertical = Input.GetAxis("Vertical");//垂直轴

        m_Movement.Set(horizontal, 0f, vertical);//把两个变量合并，y轴设为0
        m_Movement.Normalize();//位移距离单位化

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);//是否有水平位移
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);//是否有垂直位移
        bool isWalking = hasHorizontalInput || hasVerticalInput;//是否有位移
        //第一个参数是需要设置值的Animator参数的名称，第二个参数是需要设置为的值
        m_Animator.SetBool("IsWalking", isWalking);//使用Animator组件引用来调用SetBool方法

        if(isWalking)
        {
            if(!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }

        /* 此代码创建一个名为 desiredForward 的 Vector3 变量。
         * 该变量设置为名为 RotateTowards 的方法的返回值，这个方法是 Vector3 类中的一个静态方法。RotateTowards 接受四个参数：前两个是 Vector3，分别是旋转时背离和朝向的矢量。
         * 该代码以 transform.forward 开头，目标是 m_Movement 变量。transform.forward 是访问 Transform 组件并获取其前向矢量的快捷方式。
         * 接下来的两个参数是起始矢量和目标矢量之间的变化量：首先是角度变化（以弧度为单位），然后是大小变化。此代码中的角度变化为 turnSpeed * Time.deltaTime，而大小变化为 0。
         * Time.deltaTime 是距上一帧的时间（也可以将其视为两帧之间的时间）。乘它可以实现每秒进行改变而不是每帧进行改变*/
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);//该行代码仅调用 LookRotation 方法，并在给定参数的方向上创建旋转。
    }

    /*该角色有一段有趣的 Walk 动画，最好为此使用根运动。但是，该动画中没有任何旋转，
     * 如果尝试在 Update 方法中旋转刚体 (Rigidbody)，则动画可能会覆盖该刚体（这可能导致角色在应该旋转的时候不旋转）
     * 实际需要的是动画的一部分根运动，但不是全部的根运动；具体来说，需要应用移动而不是旋转。*/
    void OnAnimatorMove()//此方法允许我们根据需要应用根运动，这意味着可以分别应用移动和旋转。
    {
        /*首先，使用对 Rigidbody 组件的引用来调用其 MovePosition 方法，并传入唯一的参数：其新位置。
         * 新位置从刚体的当前位置开始，然后在此基础上添加一个更改 — 移动矢量乘以 Animator 的 deltaPosition 的大小。
         * Animator 的 deltaPosition 是由于可以应用于此帧的根运动而导致的位置变化。
         * 将其大小（即长度）乘以我们希望角色移动的实际方向上的移动向量。*/
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation(m_Rotation);//同上，但适用于旋转。这次无需进行更改，直接设置即可
    }
}
