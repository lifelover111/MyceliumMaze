#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MBS.View.Input.GUI
{
    public  class UIElement
    {
        public  enum ElementType
        {
            Label,
            Toggle,
            Dropdown,
            FloatInput,
            IntegerInput
        }

        public  ElementType type;


        public  KeyCode Key { get; set; }
        public  string Label { get; set; }
        public  Action KeyAction { get; set; }

        public  UIElement( )
        {
            type = ElementType.Label;
        }
    }

    public  class UIToggleElementData : UIElement
    {
        public  string OnText { get; set; }
        public  string OffText { get; set; }
        public  Func<bool> GetValueRemote { get; set; }
        public  Action<bool> OnValueChangeAction { get; set; }


        public  UIToggleElementData( )
        {
            type = ElementType.Toggle;
        }
    }

    public  class UIDropdownElement : UIElement
    {
        public  List<string> Choices { get; set; }
        public  Func<string> GetValueRemote { get; set; }
        public  Action<string> OnValueChangeAction { get; set; }


        public  UIDropdownElement( )
        {
            type = ElementType.Dropdown;
        }
    }

    public  class UIFloatInputData : UIElement
    {
        public  Func<float> GetValueRemote { get; set; }
        public  Action<float> OnValueChangeAction { get; set; }
        public  bool AddValuePointer { get; set; }
        public  bool Disabled { get; set; }

         
         
        public  Func<float> GetLinkedValue { get; set; }
        public  Action<float> SetNewValueRemote { get; set; }

        public  UIFloatInputData( )
        {
            type = ElementType.FloatInput;
        }
    }

    public  class UIIntegerInputData : UIElement
    {
        public  Func<int> GetValueRemote { get; set; }
        public  Action<int> OnValueChangeAction { get; set; }

        public  UIIntegerInputData( )
        {
            type = ElementType.IntegerInput;
        }
    }
}

#endif