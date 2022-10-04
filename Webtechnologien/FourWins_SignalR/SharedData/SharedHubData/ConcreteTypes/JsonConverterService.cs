//-----------------------------------------------------------------------
// <copyright file="JsonConverterService.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl, Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.SharedHubData.ConcreteTypes
{
    using Newtonsoft.Json;
    using SharedData.SharedHubData.Interfaces;
    using System;
    using System.Threading.Tasks;
    using SharedData.Exceptions;

    /// <summary>
    /// This class represents a converter which is able to convert objects to and from JSON strings.
    /// </summary>
    public class JsonConverterService : IMessageFormatConverterService<string>
    {
        /// <summary>
        /// Gets the json serializer settings used when serializing with this service.
        /// </summary>
        public readonly JsonSerializerSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConverterService"/> class and modifies the <see cref="JsonSerializerSettings"/>
        /// used when serializing with this service.
        /// </summary>
        public JsonConverterService()
        {
            this.settings = new JsonSerializerSettings();
            this.settings.TypeNameHandling = TypeNameHandling.All;
        }

        /// <summary>
        /// Converts an object into a JSON string.
        /// </summary>
        /// <param name="data">The object that is to be converted.</param>
        /// <returns>The converted object.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if object is null.
        /// </exception>
        /// <exception cref="FormatConversionException">
        /// Is thrown if conversion into a JSON string failed.
        /// </exception>
        public Task<string> ConvertAsync(object data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Object to convert must not be null.");

            return Task.Run<string>(() =>
            {
                try
                {
                    return JsonConvert.SerializeObject(data, settings);
                }
                catch (Exception e)
                {
                    throw new FormatConversionException("An error occurred during converting object to JSON string", e);
                }
            });
        }

        /// <summary>
        /// Converts a JSON string to the type specified in the generic type parameter.
        /// </summary>
        /// <typeparam name="TResult">The target type of the formatted object, after converting it back.</typeparam>
        /// <param name="formattedObject">The JSON string which needs to be converted into the type specified 
        /// by the generic type parameter.</param>
        /// <returns>The converted result.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if formatted object is null.
        /// </exception>
        /// <exception cref="FormatConversionException">
        /// Is thrown if converting back into an object of the specified type fails.
        /// </exception>
        public Task<TResult> ConvertBackAsync<TResult>(string formattedObject)
        {
            if (formattedObject == null)
                throw new ArgumentNullException(nameof(formattedObject), "JSON object to convert back must not be null.");

            return Task.Run<TResult>(() =>
            {
                try
                {
                    return JsonConvert.DeserializeObject<TResult>(formattedObject);
                }
                catch (Exception e)
                {
                    throw new FormatConversionException("An error occurred during converting object to JSON string", e);
                }
            });
        }

        /// <summary>
        /// Determines whether this converter is able to convert a formatted object back into a concrete type.
        /// </summary>
        /// <typeparam name="TResult">The type to check for conversion compatibility.</typeparam>
        /// <param name="formattedObject">The formatted object.</param>
        /// <returns>A task handling the logic and containing whether the conversion is possible in its result on termination.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if formatted object is null.
        /// </exception>
        public Task<bool> CanConvertInto<TResult>(string formattedObject)
        {
            if (formattedObject == null)
                throw new ArgumentNullException(nameof(formattedObject), "JSON object to convert back must not be null.");

            return Task<bool>.Run(() =>
            {
                try
                {
                    JsonConvert.DeserializeObject<TResult>(formattedObject);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
    }
}
