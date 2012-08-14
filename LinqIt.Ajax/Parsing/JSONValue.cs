using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LinqIt.Ajax.Parsing
{
    public abstract class JSONValue
    {
        // Methods
        protected JSONValue()
        {
        }

        public static readonly JSONNull Null = JSONNull.Create();

        public static JSONValue Parse(string value)
        {
            Token token = new Token(value);
            return Parse(token);
        }

        protected static JSONValue Parse(Token token)
        {
            JSONValue result = JSONString.Parse(token);
            if (result != null)
            {
                return result;
            }
            result = JSONBoolean.Parse(token);
            if (result != null)
            {
                return result;
            }
            result = JSONNull.Parse(token);
            if (result != null)
            {
                return result;
            }
            result = JSONNumber.Parse(token);
            if (result != null)
            {
                return result;
            }
            result = JSONObject.Parse(token);
            if (result != null)
            {
                return result;
            }
            return JSONArray.Parse(token);
        }

        public static JSONValue Serialize(object obj)
        {
            if (obj == null)
                return JSONValue.Null;
            else if (obj is JSONValue)
                return (JSONValue)obj;
            else if (obj is string)
                return new JSONString((string)obj);
            else if (obj is decimal)
                return new JSONNumber((decimal)obj);
            else if (obj is bool)
                return new JSONBoolean((bool)obj);
            else if (obj is int)
                return new JSONNumber((int)obj);
            else if (obj is DateTime)
                return new JSONDateTime((DateTime)obj);
            else if (obj is IDictionary)
            {
                JSONObject jo = new JSONObject();
                IDictionary lookup = (IDictionary)obj;
                foreach (var key in lookup.Keys)
                    jo.AddValue(key.ToString(), JSONValue.Serialize(lookup[key]));
                return jo;
            }
            else if (obj is IEnumerable)
            {
                JSONArray array = new JSONArray();
                foreach (object child in (IEnumerable)obj)
                    array.AddValue(Serialize(child));
                return array;
            }
            else
            {
                JSONObject jo = new JSONObject();
                Type type = obj.GetType();
                foreach (var property in type.GetProperties().Where(p => p.CanRead))
                {
                    jo.AddValue(property.Name, Serialize(property.GetValue(obj, null)));
                }
                return jo;
            }
            throw new ArgumentException("The object could not be serialized");
        }

        public abstract object Value { get; }

        public static implicit operator int(JSONValue value)
        {
            if (value is JSONNumber)
            {
                var number = (JSONNumber) value;
                return Convert.ToInt32(number.Value);
            }
            throw new ArgumentException("Cannot convert a " + value.GetType().Name + " to an integer");
        }

        public static implicit operator long(JSONValue value)
        {
            if (value is JSONNumber)
            {
                var number = (JSONNumber) value;
                if (number.IsDecimal)
                    throw new ArgumentException("This number is a decimal");

                return (long) number.Value;
            }
            throw new ArgumentException("Cannot convert a " + value.GetType().Name + " to an integer");
        }

        public static implicit operator decimal(JSONValue value)
        {
            if (value is JSONNumber)
            {
                var number = (JSONNumber)value;
                if (number.IsDecimal)
                    return (decimal) number.Value;

                return Convert.ToDecimal((long) number.Value);
            }
            throw new ArgumentException("Cannot convert a " + value.GetType().Name + " to an integer");
        }

        public static explicit operator string(JSONValue value)
        {
            if (value == null || value == Null)
                return null;
            if (value is JSONString)
            {
                return value.Value.ToString();
            }
            throw new ArgumentException("Cannot convert a " + value.GetType().Name + " to a string");
        }

        public static implicit operator bool(JSONValue value)
        {
            if (value == null)
                return false;

            if (value is JSONBoolean)
            {
                return (bool)value.Value;
            }
            throw new ArgumentException("Cannot convert a " + value.GetType().Name + " to a boolean");
        }
    }
}
