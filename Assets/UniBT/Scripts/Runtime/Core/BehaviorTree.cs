using UnityEngine;

namespace UniBT
{
    public enum UpdateType
    {
       Auto,
       Manual
    }
    public class BehaviorTree : MonoBehaviour
    {
        
        [HideInInspector]
        [SerializeReference]
        //private Root root = new Root();
        private Root runtimeRoot;
        private Root editorRoot;

        [SerializeField]
        RootSO rootSO;

        [SerializeField]
        private UpdateType updateType;

        public Root Root
        {
            get
            {
                if (rootSO)
                    return rootSO.GetRoot();
                else
                    return null;
            }
#if UNITY_EDITOR
            set => rootSO.SetRoot(value);
#endif
        }

        private void OnValidate()
        {
            if (!rootSO)
            {
                Debug.LogWarning($"No Root assigned to {name}");
                return;
            }

            editorRoot = rootSO.GetRoot();
        }

        private void Awake() {
            if(!rootSO)
                Debug.LogError("No Root assigned to this Behavior Tree", this);

            if (!rootSO.HasRootReference())
                Debug.LogError($"Something went wrong with the RootSO on {this}", rootSO);

            runtimeRoot = editorRoot.CloneObject() as Root;

            runtimeRoot.Run(gameObject);
            runtimeRoot.Awake();
        }

        private void Start()
        {
            runtimeRoot.Start();
        }

        private void Update()
        {
            if (updateType == UpdateType.Auto) Tick();
        }
        
        public void Tick()
        {
            runtimeRoot.PreUpdate();
            runtimeRoot.Update();
            runtimeRoot.PostUpdate();
        }

    }
}