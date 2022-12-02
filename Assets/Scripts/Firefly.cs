using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firefly : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]

    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private bool fireflyIsHit;
    // Start is called before the first frame update
    void Start()
    {
        fireflyIsHit = false;
    }
    public void LoadData(GameData data)
    {
        data.fireflysHit.TryGetValue(id, out fireflyIsHit);
        if (fireflyIsHit)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.fireflysHit.ContainsKey(id))
        {
            data.fireflysHit.Remove(id);
        }
        data.fireflysHit.Add(id, fireflyIsHit);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !fireflyIsHit)
        {
            Debug.Log("Firefly");
            fireflyIsHit = true;
            other.GetComponent<FrogCharacter>().AddFirefly(this.gameObject);
            GameManager.instance.Alert("Firefly Collected!");
        }
    }
}
