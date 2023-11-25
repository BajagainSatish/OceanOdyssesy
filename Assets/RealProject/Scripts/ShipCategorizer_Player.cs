using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCategorizer_Player : MonoBehaviour
{
    private static List<ShipCategorizer_Player> p1ShipList = new List<ShipCategorizer_Player>();
    private static List<ShipCategorizer_Player> p2ShipList = new List<ShipCategorizer_Player>();

    public bool isP1Ship;
    public bool isFunctionalShip;//not annihilated/destroyed by enemy attack.

    private HealthSystem healthSystemScript;
    private int currentShipHealth;

    public static List<ShipCategorizer_Player> GetPlayer1ShipList()
    {
        return p1ShipList;
    }

    public static List<ShipCategorizer_Player> GetPlayer2ShipList()
    {
        return p2ShipList;
    }

    private void Awake()
    {
        if (isP1Ship)
        {
            AddP1ShipToP1ShipList();
        }
        else
        {
            AddP2ShipToP2ShipList();
        }
        healthSystemScript = GetComponent<HealthSystem>();
    }
    private void Start()
    {
        // Check if each ship's 0th child is ShipCenter
        if (transform.childCount > 0 && transform.GetChild(0).name != "ShipCenter")
        {
            Debug.LogWarning("Ensure that 0th child is ShipCenter for proper distance calculation between ships!!!" + this.name);
        }
        isFunctionalShip = true;
    }

    private void Update()
    {
        TestP1P2ShipList();
        CheckShipHealth();//Later, make sure this check occurs only when ship receives damage, improve perfornamce.
    }
    private void AddP1ShipToP1ShipList()
    {
        p1ShipList.Add(this);
    }
    private void AddP2ShipToP2ShipList()
    {
        p2ShipList.Add(this);
    }
    private void PrintShipsInList(List<ShipCategorizer_Player> shipList, string listName)
    {
        Debug.Log($"{listName}:");

        foreach (var ship in shipList)
        {
            Debug.Log(ship.gameObject.name);
        }
    }
    private void TestP1P2ShipList()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PrintShipsInList(GetPlayer1ShipList(), "Player 1 Ship List");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PrintShipsInList(GetPlayer2ShipList(), "Player 2 Ship List");
        }
    }
    private void CheckShipHealth()
    {
        currentShipHealth = healthSystemScript.currentHealth;
        if (currentShipHealth <= 0)
        {
            isFunctionalShip = false;
        }
        else
        {
            isFunctionalShip = true;
        }
    }
}
