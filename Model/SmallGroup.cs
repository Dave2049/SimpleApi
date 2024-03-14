namespace SimpleApi.Models{



public record SmallGroupRequest{
    int id;
    Adress Adress;
    int maxCapacity;
}

public record Adress{
    String street;
    int postalCode;
    String city;
    String houseNumber;
}

}