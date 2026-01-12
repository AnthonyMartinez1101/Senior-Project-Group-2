using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public GameObject[] buttons;

    private Inventory inventory;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInventory(Inventory inv)
    {
        inventory = inv;
    }

    public void UpdateShopUI()
    {

    }
}
