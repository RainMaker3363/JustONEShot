using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class InAppPurchaser : MonoBehaviour, IStoreListener
{
    private static IStoreController storeController;
    private static IExtensionProvider extensionProvider;

    [HideInInspector]
    private GameObject WaitRoom;

    #region 상품ID
    // 상품ID는 구글 개발자 콘솔에 등록한 상품ID와 동일하게 해주세요.
    public const string Pro_Upgrade_productId1 = "pro_upgrade";
    #endregion

    void Start()
    {
        if(WaitRoom == null)
        {
            WaitRoom = GameObject.Find("WaitRoomSet");
        }

        InitializePurchasing();
    }

    private bool IsInitialized()
    {
        return (storeController != null && extensionProvider != null);
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
            return;

        //var module = StandardPurchasingModule.Instance();

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance()); //ConfigurationBuilder.Instance(module);

        builder.AddProduct(Pro_Upgrade_productId1, ProductType.Consumable, new IDs
        {
            { Pro_Upgrade_productId1, AppleAppStore.Name },
            { Pro_Upgrade_productId1, GooglePlay.Name },
        });

        UnityPurchasing.Initialize(this, builder);
    }

    // 실질적으로 ID를 전달하여 Purchase 처리를 해주는 함수
    // ProcessPurchase를 콜백한다.
    public void BuyProductID(string productId)
    {
        try
        {
            if (IsInitialized())
            {
                Product p = storeController.products.WithID(productId);

                if (p != null && p.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", p.definition.id));
                    storeController.InitiatePurchase(p);
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        catch (Exception e)
        {
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }

    public void RestorePurchase()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = extensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions
                (
                    (result) => { Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore."); }
                );
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController sc, IExtensionProvider ep)
    {
        Debug.Log("OnInitialized : PASS");

        storeController = sc;
        extensionProvider = ep;
    }

    public void OnInitializeFailed(InitializationFailureReason reason)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + reason);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

        switch (args.purchasedProduct.definition.id)
        {
            case Pro_Upgrade_productId1:
                {
                    //GameInfoManager.GetInstance().SetVIP();
                    if (WaitRoom == null)
                    {
                        WaitRoom = GameObject.Find("WaitRoomSet");
                        WaitRoom.GetComponent<WaitRoom>().VIPSetting();
                    }
                    else
                    {
                        WaitRoom.GetComponent<WaitRoom>().VIPSetting();
                    }

                }
                break;
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}