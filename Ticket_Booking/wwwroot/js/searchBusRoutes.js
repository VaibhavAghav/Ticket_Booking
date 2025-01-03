$(document).ready(function () {
    $('#searchButton').click(function (e) {
        e.preventDefault();

        var fromCityId = $('#FromCityId').val();
        var toCityId = $('#ToCityId').val();
        var date = $('#Date').val(); 

      
        $('#loading').show();
        $('#searchResult').hide();

        $.ajax({
            type: 'POST',
            url: '/Book/SearchBusRoutes',
            data: {
                fromCityId: fromCityId,
                toCityId: toCityId,
                bookingDateTime: date 
            },
            success: function (response) {
                var table = '<table class="table table-striped"><thead><tr><th>Bus Number</th><th>Start Time</th><th>Reached Time</th><th>Start City</th><th>Destination City</th><th>Action</th></tr></thead><tbody>';

                if (response.length > 0) {
                    response.forEach(function (route) {
                        var bookingDateTime = new Date(date);
                        var startTime = new Date(route.startTime);
                        var reachedTime = new Date(route.reachedTime);

                        startTime.setFullYear(bookingDateTime.getFullYear(), bookingDateTime.getMonth(), bookingDateTime.getDate());
                        reachedTime.setFullYear(bookingDateTime.getFullYear(), bookingDateTime.getMonth(), bookingDateTime.getDate());

                        var formattedStartTime = startTime.toLocaleString('en-US', { hour12: false });
                        var formattedReachedTime = reachedTime.toLocaleString('en-US', { hour12: false });

                        table += '<tr><td>' + route.busNumber +
                            '</td><td>' + formattedStartTime +
                            '</td><td>' + formattedReachedTime +
                            '</td><td>' + route.startCity +
                            '</td><td>' + route.destinationCity +
                            '</td><td class="action-column">';

                        if (route.isDeparted) {
                            table += 'Bus is departed';
                        } else {
                            table += '<form action="/Book/BookBus" method="get">' +
                                '<input type="hidden" name="id" value="' + route.id + '" />' +
                                '<input type="hidden" name="busNumber" value="' + route.busNumber + '" />' +
                                '<input type="hidden" name="startTime" value="' + route.startTime + '" />' +
                                '<input type="hidden" name="reachedTime" value="' + route.reachedTime + '" />' +
                                '<input type="hidden" name="startCity" value="' + route.startCity + '" />' +
                                '<input type="hidden" name="destinationCity" value="' + route.destinationCity + '" />' +
                                '<button type="submit" class="btn btn-primary">Book</button>' +
                                '</form>';
                        }

                        table += '</td></tr>';
                    });
                } else {
                    table += '<tr><td colspan="6" class="text-center">No bus routes found.</td></tr>';
                }

                table += '</tbody></table>';
                $('#searchResult').html(table);


                $('#loading').hide();
                $('#searchResult').show();
            },
            error: function (xhr, status, error) {
                console.error('Error:', status, error);
                $('#searchResult').html('<div class="alert alert-danger">An error occurred while searching for bus routes.</div>');


                $('#loading').hide();
                $('#searchResult').show();
            }
        });
    });
});


