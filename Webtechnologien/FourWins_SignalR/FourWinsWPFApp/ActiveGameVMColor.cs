//-----------------------------------------------------------------------
// <copyright file="GameVM.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Tamara Mayer</author>
//-----------------------------------------------------------------------
using System.Windows.Media;

namespace FourWinsWPFApp
{
    public static class ActiveGameVMColor
    {
        public static SolidColorBrush StandardColor_TabHeader
        {
            get
            {
                return new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        public static SolidColorBrush NotifyColor_TabHeader
        {
            get
            {
                return new SolidColorBrush(Color.FromRgb(254, 216, 177));
            }
        }
    }
}
