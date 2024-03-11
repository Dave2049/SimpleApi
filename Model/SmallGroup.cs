namespace SimpleApi.Models{

public record SmallGroupDTO{
    int id;
    String geoCode;
    Adress adress;
    int maxCapacity;
};

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