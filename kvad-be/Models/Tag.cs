public class Tag{
    public string Id { get; set; }
    public Device Device { get; set; }

    //Value can be a string, number, boolean, or object
    public object Value { get; set; }
    public Unit unit { get; set; }

}