var getUsers;
var getEvents;
var getCurrentUser;


function getAllUsers() {
    

    var webMethod = "AccountServices.asmx/GetAllAccountInfo";
    var parameters = "{}";

    //jQuery ajax method
    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (array) {
            console.log("getAllUsers started");           
            getUsers = array.d;
           
            console.log(getUsers);
                     
        },
            error: function (e) {
                alert("boo...");
            }       
        
    });
    
 
}

function getAllEvents() {

    var webMethod = "AccountServices.asmx/ViewEvents";
    console.log('Function started');
    var parameters = "{}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (array) {

            
            getEvents = array.d;
            console.log(getEvents);

           

        }

    });

}


function getCurrentUserData() {
    var webMethod = "AccountServices.asmx/ViewAccountInfo";
    var parameters = "{}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (array) {
            console.log('Function started');
            console.log(array);

            getCurrentUser = array.d;

        }
    });
}

