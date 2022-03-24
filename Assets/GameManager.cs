using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject prefab;
    public List<GameObject> playerCharacters = new();
    
    //플레이어 스폰 타이밍은 적절히 해주자.
    public void SpawnPlayer(string playerId)
    {
        foreach (var go in playerCharacters)
        {
            if (go.name == playerId)
            {
                Debug.Log("Player Already Spawned");
                return;
            }
        }
        
        Debug.Log(playerId + "플레이어를 생성");
        var player = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        print(player.transform);
        player.name = playerId;
        playerCharacters.Add(player);
        Debug.Log("New Player Spawned");
    }
}