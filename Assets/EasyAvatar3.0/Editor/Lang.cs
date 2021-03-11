using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EasyAvatar
{
    public class Lang
    {
        public static string 
            ExpressionMenu,
            ExpressionMenuNew,
            ExpressionMenuAdd,
            ExpressionMenuEdit,
            ExpressionMenuDel,
            ExpressionMenuNotEditing;

        public enum Type
        {
            En,
            Zh
        }

        public static void Change(Type type)
        {
            switch (type)
            {
                case Type.En:
                    EnglishLang.Use();
                    break;
                case Type.Zh:
                    ChineseLang.Use();
                    break;
                default:
                    break;
            }
        }
    }

    public class ChineseLang : Lang
    {
        public static void  Use()
        {
            ExpressionMenu = "菜单";
            ExpressionMenuNew = "新建菜单";
            ExpressionMenuAdd = "添加";
            ExpressionMenuEdit = "编辑";
            ExpressionMenuDel = "删除";
            ExpressionMenuNotEditing = "没有正在编辑的菜单";
        }
    }

    public class EnglishLang : Lang
    {
        public static void Use()
        {
            ExpressionMenu = "Expression Menu";
            ExpressionMenuNew = "New Menu";
            ExpressionMenuAdd = "Add Menu";
            ExpressionMenuEdit = "Edit";
            ExpressionMenuDel = "Delete";
            ExpressionMenuNotEditing = "There is no expression menu being edited";
        }
    }
}