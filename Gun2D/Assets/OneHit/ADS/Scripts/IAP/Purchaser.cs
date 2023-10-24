using System;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using System.Collections;

namespace OneHit
{
    public class Purchaser : MonoBehaviour, IStoreListener
    {
        private static IStoreController m_StoreController;
        private static IAppleExtensions m_AppleExtensions;

        private List<string> _prices = new List<string>();

        public void Init()
        {
            if (m_StoreController == null)
            {
                InitializePurchasing();
            }
        }

        private bool IsInitialized()
        {
            return m_StoreController != null;
        }

        private void InitializePurchasing()
        {
            if (IsInitialized()) return;

            Debug.Log("<color=orange> Purchaser: init </color>");

#if UNITY_EDITOR
            Debug.Log("<color=lime> Purchaser use Fake Store UI Mode!</color>");
            StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#endif

            string[] productKeys = MasterControl.Instance.productKeys;
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(productKeys[0], ProductType.NonConsumable);

            for (int i = 1; i < productKeys.Length; i++)
            {
                builder.AddProduct(productKeys[i], ProductType.Consumable);
            }

            UnityPurchasing.Initialize(CodelessIAPStoreListener.Instance, builder);
            StartCoroutine(DoWaitForInit());
        }

        private IEnumerator DoWaitForInit()
        {
            yield return new WaitUntil(() => CodelessIAPStoreListener.initializationComplete);
            OnInitialized(CodelessIAPStoreListener.Instance.StoreController, null);
            CheckRefund();
        }

#if UNITY_IOS || UNITY_EDITOR
        // private float timer = 0;
        // private bool isProcessing = false;
        // private float timer2 = 0;
        // private bool checkRestore = false;

        //void Update() {
        //    if (!isProcessing && CodelessIAPStoreListener.Instance.isPurchasing) {
        //        isProcessing = true;
        //        Controller.Instance.pleaseWaitPanel.SetActive(true);
        //        MasterControl.Instance.CheckInternet(action => {
        //            if (!action) {
        //                Controller.Instance.pleaseWaitPanel.SetActive(false);
        //                isProcessing = false;
        //                CodelessIAPStoreListener.Instance.isPurchasing = false;
        //            }
        //        }, true);

        //    }
        //    else if (isProcessing && !CodelessIAPStoreListener.Instance.isPurchasing) {
        //        if (Controller.Instance.pleaseWaitPanel.activeInHierarchy) {
        //            Controller.Instance.pleaseWaitPanel.SetActive(false);
        //        }
        //        isProcessing = false;
        //    }

        //    if (CodelessIAPStoreListener.Instance.didRestoreSuccess) {
        //        CodelessIAPStoreListener.Instance.didRestoreSuccess = false;
        //        MessagePanel.Instance.SetUp("Restore successfully", "MESSAGE");
        //        Debug.Log("Restore Success");
        //        CheckRestore();
        //    }
        //}
#endif

        private void CheckRefund()
        {
            Debug.LogWarning("PURCHASER: Check refund!");

            Debug.Log("PRODUCT: " + MasterControl.Instance.productKeys[0] + ": " + m_StoreController.products.WithID(MasterControl.Instance.productKeys[0]).hasReceipt);
            if (!m_StoreController.products.WithID(MasterControl.Instance.productKeys[0]).hasReceipt && !PrefInfo.IsUsingAd())
            {
                Debug.LogError("PURCHASER: User buy pack RemoveAds not success => Reset ads!");
                PrefInfo.SetAd(true);
            }
        }

        public void CheckRestore()
        {
            Debug.LogWarning("Purchaser: Check restore!");
            if (m_StoreController != null)
            {
                Debug.Log("PRODUCT: " + MasterControl.Instance.productKeys[0] + ": " + m_StoreController.products.WithID(MasterControl.Instance.productKeys[0]).hasReceipt);
                bool check = false;
                if (m_StoreController.products.WithID(MasterControl.Instance.productKeys[0]).hasReceipt)
                {
                    check = true;
                    MasterControl.Instance.OnPurchased(MasterControl.Instance.productKeys[0]);
                }
                if (!check)
                {
                    MessagePanel.Instance.Show();
                }
            }
        }

        public bool HasReceipt(string id)
        {
            if (m_StoreController != null)
            {
                if (m_StoreController.products.WithID(id).hasReceipt)
                {
                    return true;
                }
            }
            return false;
        }

        public void BuyProductID(string productId)
        {
            if (IsInitialized())
            {
                Product product = m_StoreController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    m_StoreController.InitiatePurchase(product);
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

        public void RestorePurchases()
        {
            if (!IsInitialized())
            {
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            #region use for 4.5.2 or older version
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                Debug.Log("RestorePurchases started ...");

                var apple = CodelessIAPStoreListener.Instance.GetStoreExtensions<IAppleExtensions>();

                apple.RestoreTransactions((result) =>
                {
                    Debug.Log("Restore purchases continue: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            else
            {
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
            #endregion
        }

        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized: PASS");
            m_StoreController = controller;
            //m_StoreExtensionProvider = extensions;
            m_AppleExtensions = CodelessIAPStoreListener.Instance.GetStoreExtensions<IAppleExtensions>();
            CheckSubscription();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }

        public List<string> GetPrices()
        {
            foreach (var product in m_StoreController.products.all)
            {
                Debug.Log(product.metadata.localizedTitle + " " + product.metadata.localizedPriceString + " " + product.metadata.localizedPrice);
                _prices.Add(product.metadata.localizedPriceString);
            }
            return _prices;
        }

        public decimal GetPrice(int id)
        {
            try
            {
                return m_StoreController.products.all[id].metadata.localizedPrice;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return 0;
            }
        }

        public bool CheckSubscription()
        {
            Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
            // Sample code for expose product sku details for apple store
            //Dictionary<string, string> product_details = m_AppleExtensions.GetProductDetails();

            Debug.Log("Available items:");
            foreach (var item in m_StoreController.products.all)
            {
                if (item.availableToPurchase)
                {
                    Debug.Log(string.Join(" - ", new[]
                    {
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedPriceString,
                        item.transactionID,
                        item.receipt
                    }));
#if INTERCEPT_PROMOTIONAL_PURCHASES
                // Set all these products to be visible in the user's App Store according to Apple's Promotional IAP feature
                // https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/StoreKitGuide/PromotingIn-AppPurchases/PromotingIn-AppPurchases.html
                m_AppleExtensions.SetStorePromotionVisibility(item, AppleStorePromotionVisibility.Show);
#endif
                    // this is the usage of SubscriptionManager class\
                    if (item.receipt != null)
                    {
                        if (item.definition.type == ProductType.Subscription)
                        {
                            Debug.Log("CHECK SUBSCRIPTION :" + item.metadata.localizedTitle + " :: " + checkIfProductIsAvailableForSubscriptionManager(item.receipt));
                            if (checkIfProductIsAvailableForSubscriptionManager(item.receipt))
                            {
                                string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                                SubscriptionManager p = new SubscriptionManager(item, intro_json);
                                SubscriptionInfo info = p.getSubscriptionInfo();
                                Debug.Log("product id is: " + info.getProductId());
                                Debug.Log("purchase date is: " + info.getPurchaseDate());
                                Debug.Log("subscription next billing date is: " + info.getExpireDate());
                                Debug.Log("is subscribed? " + info.isSubscribed().ToString());
                                Debug.Log("is expired? " + info.isExpired().ToString());
                                Debug.Log("is cancelled? " + info.isCancelled());
                                Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
                                Debug.Log("product is auto renewing? " + info.isAutoRenewing());
                                Debug.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                                Debug.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                                Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                                Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                                Debug.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());

                                //neu nhu da het han
                            }
                            else
                            {
                                Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                            }
                        }
                        else
                        {
                            Debug.Log("the product is not a subscription product");
                        }
                    }
                    else
                    {
                        Debug.Log("the product should have a valid receipt " + item.definition.id);
                    }
                }
            }
            return false;
        }

        private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
        {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt)
    ;
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
            {
                Debug.Log("The product receipt does not contain enough information");
                return false;
            }
            var store = (string)receipt_wrapper["Store"];
            var payload = (string)receipt_wrapper["Payload"];

            if (payload != null)
            {
                switch (store)
                {
                    case GooglePlay.Name:
                        {
                            var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                            if (!payload_wrapper.ContainsKey("json"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                                return false;
                            }
                            var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                            if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                                return false;
                            }
                            var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                            var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                            if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                                return false;
                            }
                            return true;
                        }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name:
                        {
                            return true;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
            return false;
        }

        public void OnInitializeFailed(InitializationFailureReason reason, string mess)
        {

        }
    }
}