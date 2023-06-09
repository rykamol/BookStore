var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "city", "width": "15%" },
            { "data": "postalCode", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="w-75 btn-group" role="group">
							
							<a class="btn btn-primary mx-2" href="/Admin/Company/Upsert?id=${data}">
								<i class="bi bi-pencil-square"></i> <br> Edit
							</a>
							
                            <a onClick=Delete('/Admin/Company/Delete/'+${data}) class="btn btn-danger mx-2">
								<i class="bi bi-trash-fill"></i> <br>Delete
							</a>
						    </div>
                    `;
                },
                "width": "15%"
            },
        ]
    })
}



function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover this imaginary file!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message)
                    } else {
                        toastr.error(data.message)

                    }
                }
            })
        }
        //else {
        //    swal("Your imaginary file is safe!");
        //}
    });
}