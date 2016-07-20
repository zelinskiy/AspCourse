
function sendNewMessage(id) {
    $.post({
        url: "/Chat/AddNewMessage",
        data: {
            NewMessageText: $('#newMessageText').val(),
            NewMessageTopicId: id,
            NewMessagePictureUrl: $('#newMessagePictureUrl').val()
        },
        success: function (data) {
            console.log(data);
            if (data === "YOU ARE MUTED") {
                alert("You are muted!");
            }
            window.location.reload();
        },
    });
}



function removeMessage(id) {
    $.post({
        url: "/Chat/RemoveMessage/?id=" + id,
        success: function (data) {
            console.log(data);
            window.location.reload();
        }
    });
}

function replyToMessage(id) {
    $('#newMessageText').val($('#newMessageText').val() + '>>' + id + '\n');
}





function uploadImage() {
    var data = new FormData($("#uploadForm")[0]);

    $.ajax({
        url: "/File/AddFile",
        type: "POST",
        data: data,
        contentType: false,
        dataType: false,
        processData: false,
        success: function (result) {
            $("#newMessagePictureUrl").val("/uploads/" + result);
        }
    });
}


function sendNewTopic() {
    $.post({
        url: "/Chat/AddNewTopic",
        data: {
            NewMessageText: $('#newTopicText').val(),
            NewTopicTitle: $('#newTopicTitle').val(),
            NewMessagePictureUrl: $('#newMessagePictureUrl').val()
        },
        success: function (data) {
            window.location.reload();
            console.log(data);
        }
    });
}

function removeTopic(id) {
    $.post({
        url: "/Chat/RemoveTopic/?id=" + id,
        success: function (data) {
            console.log(data);
            window.location.reload();
        }
    });
}


function toggleTopicSticked(id) {
    $.post({
        url: "/Chat/ToggleTopicSticky/?id=" + id,
        type: "POST",
        success: function (data) {
            console.log(data);
            window.location.reload();
        }
    });
}


function toggleTopicClosed(id) {
    $.post({
        url: "/Chat/ToggleTopicClosed/?id=" + id,
        success: function (data) {
            console.log(data);
            window.location.reload();
        }
    });
}

function toggleSubscribeTopic(id) {
    toggleLike(id, "Topic", "Subscription");
}

function toggleLike(id,entity, type) {
    $.post({
        url: "/Chat/ToggleLike" + entity + "/?id=" + id +"&type=" + type ,
        success: function (data) {
            console.log(data);
            window.location.reload();
        }
    });
}
