
public static class ItemExtensions {

    public static bool isEmpty(this Item item) {
        return item.ItemSubType == ItemSubType.Empty;
    }

}
