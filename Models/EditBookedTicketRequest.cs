public class EditBookedTicketRequest
{
    public List<TicketUpdateModel> Tickets { get; set; }
}

public class TicketUpdateModel
{
    public string KodeTicket { get; set; }
    public int Quantity { get; set; }
}

