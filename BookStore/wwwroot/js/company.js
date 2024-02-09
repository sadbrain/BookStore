var dataTable;
$(document).ready(() => {
    loadDataTable();
});
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/company/getall' },
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'streetAddress', "width": "15%" },
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            {
                data: 'id',
                "width": "25%",
                "render": (data) => {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-4" >
                            <i class="bi bi-pencil-square"></i>  Edit
                        </a>

                        <a onclick=Delete('/admin/company/delete/${data}') class="btn btn-danger mx-4">
                            <i class="bi bi-trash-fill"></i>  Delete
                        </a>
                    </div > `

                }
                
            }
        ]
    });
}   

function Delete(url) {
    //hiển ra hopop thoại xác nhận xóa
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"

    }).then((result) => {
        //kiểm tra xác nhận xóa thành công
        if (result.isConfirmed) {
            //call api delete và thựn hiện hàm success
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    //reload laị bảng
                    dataTable.ajax.reload();
                    toastr.success(data.message)
                }
            })
        }
    });
}