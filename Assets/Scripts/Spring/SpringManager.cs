//
//SpingManager.cs for unity-chan!
//
//Original Script is here:
//ricopin / SpingManager.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//Revised by N.Kobayashi 2014/06/24
//           Y.Ebata
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityChan
{
    public class SpringManager : MonoBehaviour
    {
        public string boneKeyword;

        //Kobayashi
        // DynamicRatio is paramater for activated level of dynamic animation 
        public float dynamicRatio = 1.0f;

        //Ebata
        public float stiffnessForce;
        public AnimationCurve stiffnessCurve;
        public float dragForce;
        public AnimationCurve dragCurve;
        public SpringBone[] springBones;

        void Start()
        {
            UpdateParameters();
        }

        void Update()
        {
#if UNITY_EDITOR
            //Kobayashi
            if (dynamicRatio >= 1.0f)
                dynamicRatio = 1.0f;
            else if (dynamicRatio <= 0.0f)
                dynamicRatio = 0.0f;
            //Ebata
            UpdateParameters();
#endif
        }

        private void LateUpdate()
        {
            //Kobayashi
            if (dynamicRatio != 0.0f)
            {
                for (int i = 0; i < springBones.Length; i++)
                {
                    if (dynamicRatio > springBones[i].threshold)
                    {
                        springBones[i].UpdateSpring();
                    }
                }
            }
        }

        private void UpdateParameters()
        {
            UpdateParameter("stiffnessForce", stiffnessForce, stiffnessCurve);
            UpdateParameter("dragForce", dragForce, dragCurve);
        }

        private void UpdateParameter(string fieldName, float baseValue, AnimationCurve curve)
        {
            var start = curve.keys[0].time;
            var end = curve.keys[curve.length - 1].time;
            //var step	= (end - start) / (springBones.Length - 1);

            var prop = springBones[0].GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            for (int i = 0; i < springBones.Length; i++)
            {
                //Kobayashi
                if (!springBones[i].isUseEachBoneForceSettings)
                {
                    var scale = curve.Evaluate(start + (end - start) * i / (springBones.Length - 1));
                    prop.SetValue(springBones[i], baseValue * scale);
                }
            }
        }

        [ContextMenu("绑定所有骨骼")]
        public void BindAllBones()
        {
            List<SpringBone> list = new List<SpringBone>();
            FindBones(transform, list);
            springBones = list.ToArray();
        }

        public void FindBones(Transform node, List<SpringBone> bones)
        {
            if (node.childCount == 0)
                return;

            foreach (Transform child in node)
            {
                Debug.Log($"search {child.name}");
                if (child.name.Contains(boneKeyword))
                    BindBones(child, bones);

                FindBones(child, bones);
            }
        }

        private void BindBones(Transform node, List<SpringBone> bones)
        {
            if (node.childCount == 0)
                return;

            Debug.Log($"bind {node.name}");
            if (!node.TryGetComponent(out SpringBone bone))
            {
                bone = node.gameObject.AddComponent<SpringBone>();

                Transform child = node.GetChild(0);
                bone.child = child;
                bone.boneAxis = Vector3.up;
            }

            bones.Add(bone);
        }
    }
}