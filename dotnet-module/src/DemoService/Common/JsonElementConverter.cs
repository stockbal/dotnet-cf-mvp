// Copyright (c) Demo AG. All Rights Reserved.

using System.Text.Json;

namespace DevEpos.CF.Demo.Common;

public class JsonElementConverter {
    public object? ConvertJsonElementToObject(JsonElement element) {
        switch (element.ValueKind) {
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number: {
                    if (element.TryGetInt32(out var number)) {
                        return number;
                    } else {
                        return element.GetInt64();
                    }
                }
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();
            case JsonValueKind.Null:
                return null;
            case JsonValueKind.Array:
                return ConvertToObjectArray(element);
        }
        return null;
    }

    private object? ConvertToObjectArray(JsonElement element) {
        if (element.GetArrayLength() <= 0) {
            return null;
        }

        var listOfJsonArrayEntries = new List<object>();
        foreach (var item in element.EnumerateArray()) {
            var convertedElement = ConvertJsonElementToObject(item);
            if (convertedElement != null) {
                listOfJsonArrayEntries.Add(convertedElement);
            }
        }
        return listOfJsonArrayEntries.ToArray();
    }
}
