class Menu
{
    //this is a 1:1 relation, so we can use the same id
    public required User User { get; set; }
    public required MenuItem MenuItems { get; set; }


}