//-----------------------------------------------------------------------
// <copyright file="IMessageFormatConverterService.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.SharedHubData.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using SharedData.Exceptions;

    /// <summary>
    /// Defines an object capable of converting objects into message formats that are able to be transmitted.
    /// </summary>
    public interface IMessageFormatConverterService<TFormat> where TFormat : class
    {
        /// <summary>
        /// Converts an object into the format specified by the converter.
        /// </summary>
        /// <param name="obj">The object that is to be converted.</param>
        /// <returns>The converted object.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if the object is null.
        /// </exception>
        /// <exception cref="FormatConversionException">
        /// Is thrown if the formatting failed.
        /// </exception>
        Task<TFormat> ConvertAsync(object obj);

        /// <summary>
        /// Converts an object which was previously converted into the type <see cref="TFormat"/> back to the specified type.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="formattedObject">The formatted object which needs to be converted into the type specified 
        /// by the generic type parameter.</param>
        /// <returns>The reconverted object.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if formatted object is null.
        /// </exception>
        /// <exception cref="FormatConversionException">
        /// Is thrown if the formatted object could not be converted back.
        /// </exception>
        Task<TResult> ConvertBackAsync<TResult>(TFormat formattedObject);

        /// <summary>
        /// Determines whether this converter is able to convert a formatted object back into a concrete type.
        /// </summary>
        /// <typeparam name="TResult">The type to check for conversion compatibility.</typeparam>
        /// <param name="formattedObject">The formatted object.</param>
        /// <returns>A task handling the logic and containing whether the conversion is possible in its result on termination.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if formatted object is null.
        /// </exception>
        Task<bool> CanConvertInto<TResult>(string formattedObject);
    }
}
