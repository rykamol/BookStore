﻿var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprogress")) {
        loadDataTable("inprogress");
    }
    else if (url.includes("completed")) {
        loadDataTable("completed");
    }
    else if (url.includes("pending")) {
        loadDataTable("pending");
    }
    else if (url.includes("approved")) {
        loadDataTable("approved");
    }
    else {
        loadDataTable("all");
    }

    //loadDataTable();

});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll?status=" + status
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "25%" },
            { "data": "phoneNumer", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "5%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="w-75 btn-group" role="group">
							    <a class="btn btn-primary mx-2" href="/Admin/Order/Details?orderId=${data}">
								    <i class="bi bi-pencil-square"></i> <br> Details
							    </a>					
						    </div>
                    `;
                },
                "width": "5%"
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