using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using FourWinsWPFApp.Models;

namespace FourWinsWPFApp.Converter
{
    public class ChallengeStatusToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts an object of type <see cref="ChallengeStatus"/> into a color.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">Unused parameter.</param>
        /// <param name="parameter">Unused parameter</param>
        /// <param name="culture">Unused parameter</param>
        /// <returns>The color which the object was converted to.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Enum.TryParse<ChallengeStatus>(value.ToString(), out ChallengeStatus parsedValue))
                throw new ArgumentException(nameof(value), $"Value must be of type {typeof(ChallengeStatus)}");

            switch (parsedValue)
            {
                case ChallengeStatus.ChallengeIncoming:
                    return Brushes.Orange;
                case ChallengeStatus.ChallengeOutgoing:
                    return Brushes.Red;
                case ChallengeStatus.Available:
                    return Brushes.Green;
                case ChallengeStatus.CreatingMatch:
                    return Brushes.Blue;
                default:
                    throw new ArgumentException(nameof(value), "Value was not a valid Challenge status.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
