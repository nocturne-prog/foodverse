using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Marvrus
{
    public sealed class ProjectManager
    {
        private static volatile ProjectManager uniqueInstance;  // 싱글톤 인스턴스를 저장
        private static object _lock = new System.Object();      // 접근 한정자
        public static ProjectManager Instance
        {
            get
            {
                if (uniqueInstance == null)
                {
                    lock (_lock)
                    {
                        if (uniqueInstance == null)
                        {
                            uniqueInstance = new ProjectManager();
                        }
                    }
                }

                return uniqueInstance;
            }
        }

        private ProjectManager()
        {
            ProjectSetting();
        }

        private void ProjectSetting()
        {

        }
    }
}