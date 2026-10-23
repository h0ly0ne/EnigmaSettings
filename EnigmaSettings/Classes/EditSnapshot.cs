// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Krkadoni.EnigmaSettings
{
    /// <summary>
    ///     IEditableObject support for the model classes: BeginEdit captures every instance field (and the content of
    ///     list fields), CancelEdit writes them back. New properties are covered automatically, so they can't be
    ///     forgotten in BeginEdit / CancelEdit.
    /// </summary>
    internal static class EditSnapshot
    {
        // the snapshot itself must not be captured or restored
        private const string SnapshotFieldName = "_editSnapshot";

        private static readonly ConcurrentDictionary<Type, FieldInfo[]> FieldCache = new();

        public static object[] Capture(object owner)
        {
            FieldInfo[] fields = FieldsOf(owner.GetType());
            var values = new object[fields.Length * 2];

            for (int i = 0; i < fields.Length; i++)
            {
                object value = fields[i].GetValue(owner);
                values[i * 2] = value;

                // lists are restored to their content, not only to the same list instance
                if (value is IList list and not Array)
                    values[i * 2 + 1] = list.Cast<object>().ToArray();
            }

            return values;
        }

        public static void Restore(object owner, object[] values)
        {
            FieldInfo[] fields = FieldsOf(owner.GetType());

            for (int i = 0; i < fields.Length; i++)
            {
                object value = values[i * 2];
                fields[i].SetValue(owner, value);

                if (value is IList list && values[i * 2 + 1] is object[] items)
                {
                    list.Clear();
                    foreach (var item in items)
                        list.Add(item);
                }
            }
        }

        private static FieldInfo[] FieldsOf(Type type)
        {
            return FieldCache.GetOrAdd(type, t =>
            {
                var fields = new List<FieldInfo>();
                for (var current = t; current != null && current != typeof(object); current = current.BaseType)
                {
                    fields.AddRange(current
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                        .Where(f => !f.IsInitOnly
                                    && f.Name != SnapshotFieldName
                                    && !typeof(Delegate).IsAssignableFrom(f.FieldType)));
                }

                return fields.ToArray();
            });
        }
    }
}
