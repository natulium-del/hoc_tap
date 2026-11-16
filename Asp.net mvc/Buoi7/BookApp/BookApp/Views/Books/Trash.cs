using Microsoft.EntityFrameworkCore.Metadata.Internal;

@model List<Book>

<h2>Thùng rác – sách đã xóa mềm</h2>

@if (TempData["Msg"] is string msg)
{
    < div class= "alert alert-success" > @msg </ div >
}

< table class= "table table-striped" >
    < tr >
        < th > Id </ th >
        < th > Tiêu đề </ th >
        < th class= "text-end" > Giá </ th >
        < th ></ th >
    </ tr >
    @foreach(var b in Model)
    {
        < tr >
            < td > @b.Id </ td >
            < td > @b.Title </ td >
            < td class= "text-end" > @b.Price.ToString("N0") </ td >
            < td >
                < form asp - action = "Restore" asp - route - id = "@b.Id" method = "post" class= "d-inline" >
                    < button class= "btn btn-success btn-sm" > Khôi phục </ button >
                </ form >
                < form asp - action = "HardDelete" asp - route - id = "@b.Id" method = "post" class= "d-inline" >
                    < button class= "btn btn-danger btn-sm" > Xóa vĩnh viễn</button>
                </form>
            </td>
        </tr>
    }
</ table >