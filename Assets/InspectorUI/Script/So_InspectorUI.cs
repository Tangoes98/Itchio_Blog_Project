using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "So_InspectorUI/NewInspectorUISO")]
public class So_InspectorUI : ScriptableObject
{

    public float TipRate;
    public List<Order> Orders;
    public float TotalPrice;
    public List<OutPrice> OutPrices;



    Dictionary<PlayerType, float> _priceToOrderer = new();

    public void ClearOutput()
    {
        TotalPrice = 0f;
        _priceToOrderer.Clear();
        OutPrices.Clear();
    }
    public void CheckOut()
    {
        OutPricesInit(OutPrices);

        foreach (var item in Orders)
        {
            AddIndividualPriceToDic(_priceToOrderer, item.Price, item.Orderers);
        }

        UpdateItemPricesToOrderer(_priceToOrderer, OutPrices);

        TotalPrice = GetTotalPrice(Orders);
        AddTipTax(OutPrices, 0.13f, TipRate);
        FinalizeIndividualPrice(OutPrices);
    }

    void AddIndividualPriceToDic(Dictionary<PlayerType, float> dic, float price, PlayerType[] orderers)
    {
        float individualPrice = price / orderers.Length;
        for (int i = 0; i < orderers.Length; i++)
        {
            if (dic.ContainsKey(orderers[i]))
                dic[orderers[i]] += individualPrice;
            else
                dic.Add(orderers[i], individualPrice);
        }
    }

    void OutPricesInit(List<OutPrice> outPrices)
    {
        PlayerType[] playerTypes = (PlayerType[])Enum.GetValues(typeof(PlayerType));

        for (int i = 0; i < playerTypes.Length; i++)
        {
            OutPrice op = new();
            op.OrdererName = playerTypes[i].ToString();
            op.Orderer = playerTypes[i];
            op.ItemPrice = 0f;
            outPrices.Add(op);
        }
    }

    void UpdateItemPricesToOrderer(Dictionary<PlayerType, float> dic, List<OutPrice> outPrices)
    {
        for (int i = 0; i < outPrices.Count; i++)
        {
            OutPrice temp = outPrices[i];
            if (dic.TryGetValue(temp.Orderer, out float outPrice))
                temp.ItemPrice = outPrice;
            else
                temp.ItemPrice = 0f;

            outPrices[i] = temp;
        }
    }

    float GetTotalPrice(List<Order> orders)
    {
        float outPrice = 0f;
        for (int i = 0; i < orders.Count; i++)
        {
            outPrice += orders[i].Price;
        }
        return outPrice;
    }

    void AddTipTax(List<OutPrice> outPrices, float taxRate, float tipRate)
    {
        for (int i = 0; i < outPrices.Count; i++)
        {
            if (outPrices[i].ItemPrice == 0f)
                continue;

            OutPrice temp = outPrices[i];
            temp.Tax = temp.ItemPrice * taxRate;
            temp.Tip = temp.ItemPrice * tipRate;
            outPrices[i] = temp;
        }
    }

    void FinalizeIndividualPrice(List<OutPrice> outPrices)
    {
        for (int i = 0; i < outPrices.Count; i++)
        {
            if (outPrices[i].ItemPrice == 0f)
                continue;

            OutPrice temp = outPrices[i];
            temp.FinalPrice = temp.ItemPrice + temp.Tax + temp.Tip;
            outPrices[i] = temp;
        }
    }

}


public enum PlayerType
{
    Tony, Dong, Tango, Harris, Theo, Zimu, Jeff
}

[Serializable]
public struct Order
{
    public string ItemName;
    public float Price;
    public PlayerType[] Orderers;
}

[Serializable]
public struct OutPrice
{
    public string OrdererName;
    public PlayerType Orderer;
    public float ItemPrice;
    public float Tax;
    public float Tip;
    public float FinalPrice;
}