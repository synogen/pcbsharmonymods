using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Order_Templates
{
    class CheckoutFinalRowLogic
    {

        public InputField templateName;

        public CheckoutFinalRow instance;

        public CheckoutFinalRowLogic(CheckoutFinalRow instance)
        {
            this.instance = instance;
            CreateTemplateUI();
        }

        private void CreateTemplateUI()
        {
            Text text = UnityEngine.Object.Instantiate<Text>(instance.m_total, instance.m_buyButton.transform.parent);
            text.transform.localPosition = new Vector3(instance.m_buyButton.transform.localPosition.x - 215f, instance.m_buyButton.transform.localPosition.y - 40f, instance.m_buyButton.transform.localPosition.z);
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(text.GetComponent<RectTransform>().sizeDelta.x + 50f, text.GetComponent<RectTransform>().sizeDelta.y + 0f);
            text.text = "Order Templates";
            Button button = UIUtil.CreateTemplateButton(instance.m_buyButton, "Save As Template", 50f, 0f, -120f, -70f);
            button.onClick.AddListener(new UnityAction(this.SaveTemplate));
            button.interactable = instance.m_buyButton.interactable;
            this.templateName = UIUtil.CreateInput(instance.GetComponentInParent<Shop>().m_searchInResults, instance.m_buyButton, "Template name", 70f, 0f, -360f, -70f);
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<LayoutElement>();
            gameObject.GetComponent<LayoutElement>().preferredHeight = 25f;
            gameObject.GetComponent<LayoutElement>().minHeight = 25f;
            gameObject.AddComponent<ContentSizeFitter>();
            gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            gameObject.transform.SetParent(instance.GetComponentInParent<Shop>().m_checkoutList.content.transform, false);
            getRows().Add(gameObject.transform);
            this.UpdateTemplateView(gameObject.GetComponent<LayoutElement>());
            LayoutRebuilder.MarkLayoutForRebuild(instance.m_buyButton.transform.parent as RectTransform);
        }

        private List<Component> getRows()
        {
            Type shop = typeof(Shop);
            FieldInfo field = shop.GetField("m_rows", BindingFlags.NonPublic | BindingFlags.Instance);
            return (List<Component>)field.GetValue(instance.GetComponentInParent<Shop>());
        }

        private List<ShopEntry> getTrolley()
        {
            Type shop = typeof(Shop);
            FieldInfo field = shop.GetField("m_trolley", BindingFlags.NonPublic | BindingFlags.Instance);
            return (List<ShopEntry>)field.GetValue(instance.GetComponentInParent<Shop>());
        }

        private List<ShopEntry> GetPartsForSale()
        {
            Type shop = typeof(Shop);
            FieldInfo field = shop.GetField("m_partsForSale", BindingFlags.NonPublic | BindingFlags.Instance);
            return (List<ShopEntry>)field.GetValue(instance.GetComponentInParent<Shop>());
        }

        private void UpdateTrolley()
        {
            Type shop = typeof(Shop);
            MethodInfo method = shop.GetMethod("UpdateTrolley", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(instance.GetComponentInParent<Shop>(), new object[] { });
        }


        public void SaveTemplate()
        {
            TemplateManager.Instance.AddTemplate(this.templateName.text, getTrolley());
            instance.GetComponentInParent<Shop>().OnCheckout();
        }

        public void UpdateTemplateView(LayoutElement le)
        {
            float num = -110f;
            List<string> list = TemplateManager.Instance.GetAllTemplates().Keys.ToList<string>();
            list.Sort();
            using (List<string>.Enumerator enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    string key = enumerator.Current;
                    UIUtil.CreateTemplateButton(instance.m_buyButton, "Restore " + key, 80f, 0f, -370f, num).onClick.AddListener(delegate ()
                    {
                        List<string> partIds = TemplateManager.Instance.GetTemplate(key);
                        getTrolley().Clear();
                        foreach (string partId in partIds)
                        {
                            foreach (ShopEntry entry in GetPartsForSale())
                            {
                                if (partId.Equals(entry.m_part.m_id))
                                {
                                    getTrolley().Add(entry);
                                }
                            }
                        }
                        UpdateTrolley();
                        instance.GetComponentInParent<Shop>().OnCheckout();
                    });
                    UIUtil.CreateTemplateButton(instance.m_buyButton, "Add from " + key, 80f, 0f, -185f, num).onClick.AddListener(delegate ()
                    {
                        List<string> partIds = TemplateManager.Instance.GetTemplate(key);
                        foreach (string partId in partIds)
                        {
                            foreach (ShopEntry entry in GetPartsForSale())
                            {
                                if (partId.Equals(entry.m_part.m_id))
                                {
                                    getTrolley().Add(entry);
                                }
                            }
                        }
                        UpdateTrolley();
                        instance.GetComponentInParent<Shop>().OnCheckout();
                    });
                    UIUtil.CreateTemplateButton(instance.m_buyButton, "Delete " + key, 80f, 0f, 0f, num).onClick.AddListener(delegate ()
                    {
                        TemplateManager.Instance.RemoveTemplate(key);
                        instance.GetComponentInParent<Shop>().OnCheckout();
                    });
                    num -= 40f;
                    le.preferredHeight += 30f;
                    le.minHeight += 30f;
                }
            }
        }
    }
}
