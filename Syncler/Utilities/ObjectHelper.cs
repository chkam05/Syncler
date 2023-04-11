using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Syncler.Utilities
{
    public static class ObjectHelper
    {

        //  METHODS

        #region ATTRIBUTES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if property contains attribute. </summary>
        /// <param name="property"> Property info, </param>
        /// <param name="attributeType"> Attribute type. </param>
        /// <returns> True - property contains attribute; False - otherwise. </returns>
        public static bool HasAttribute(PropertyInfo property, Type attributeType)
        {
            return Attribute.IsDefined(property, attributeType);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of attributes with specified type. </summary>
        /// <typeparam name="T"> Attribute type. </typeparam>
        /// <param name="property"> Property info, </param>
        /// <returns> List of attributes or empty array. </returns>
        public static List<T> GetAttribute<T>(PropertyInfo property) where T : Attribute
        {
            if (HasAttribute(property, typeof(T)))
            {
                var attributes = (T[])property.GetCustomAttributes(typeof(T), false);

                if (attributes != null && attributes.Any())
                    return attributes.ToList();
            }

            return new List<T>();
        }

        #endregion ATTRIBUTES METHODS

        #region PROPERTIES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get public valuable properties from class object. </summary>
        /// <typeparam name="T"> Class object type. </typeparam>
        /// <param name="obj"> Class object. </param>
        /// <returns> Array of class properties. </returns>
        public static PropertyInfo[] GetObjectProperties<T>(T obj) where T : class
        {
            return obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get public valuable properties from class type. </summary>
        /// <param name="type"> Class type. </param>
        /// <returns> Array of class properties. </returns>
        public static PropertyInfo[] GetObjectProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        #endregion PROPERTIES METHODS

    }
}
