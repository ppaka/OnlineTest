using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        private SocketIOManager socketIOManager = null;
        private int currentPlayerAnim = 0;
    
        private void Start()
        {
            socketIOManager = SocketIOManager.Instance;
        }

        private void Update()
        {
            //.... 플레이어 움직임, 애니메이션 업데이트
            //socketIOManager.SendTransformAndAnimationData(this.transform, currentPlayerAnim);
        }
        
        public void RandomPosition(){
            transform.position = new Vector3(Random.Range(0f, 4f),Random.Range(0f, 4f),0);
            socketIOManager.SendTransformAndAnimationData(this.transform, currentPlayerAnim);
        }
    }
