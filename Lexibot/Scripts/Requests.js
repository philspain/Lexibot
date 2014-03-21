/// <reference path="jquery-1.5.1.min.js" />

function Requests() {
    this.postIndex = 1;
    this.postHeight = 0;
    this.occurrenceIndex = 1;
    this.occurrenceHeight = 1;
    

    this.requestPosts = function (year, month) {
        $.post("/Requests/GetPosts", { "year": year, "month": month },
        function (data) {
            var html = '<div class="movable" id="first-posts" style="top: 0px;">';

            for (var i = 0; i < data.length; i++) {
                var id = '';

                html += '<div class="div-style-' + (requests.postIndex % 2) + '"' + id + '>'
                             + '<div>' + data[i].ThingID + '</div>'
                             + '<div>' + data[i].Text.replace(/\n/g, '<br />') + '</div>'
                             + '<div>' + data[i].PermaLink + '</div>'
                             + '<div>' + data[i].CommentDate + '</div>'
                         + '</div>';

                requests.postIndex++;
            }

            html += '</div>';

            $('#posts').html(html);

            var totalPostsHeight = 0;

            $('#first-posts').children().each(function () {
                var elem = $(this);
                totalPostsHeight += elem.height();
            });

            $('#posts').height(totalPostsHeight);

            if ($('#posts').height() > $('#content-container').height()) {
                $('#posts').append($('#first-posts').clone().attr('id', 'last-posts').css('top', $('#first-posts').height()));

                $('#last-posts').children().each(function () {
                    var elem = $(this);
                    elem.attr('class', 'div-style-' + (requests.postIndex % 2));
                    requests.postIndex++;
                });

                ui.tryScrollPosts();
            }

        }, 'json');
    }

    this.retrieveOccurrences = function () {
        var self = this;
        var occurrencesObj;

        $.ajax({
            type: 'GET',
            url: '/Requests/GetWordOccurrences',
            dataType: 'json',
            success: function (reqData) { occurrencesObj = self.parseOccurrencesJson(reqData); },
            data: {},
            async: false
        });

        return occurrencesObj;
    }

    this.parseOccurrencesJson = function (data) {
        var occurrencesObj = { count: 0, words: {} };
            
        $.each(data, function (a, b) {
            // ensure current Word exists as property on object
            if (!occurrencesObj.words.hasOwnProperty(b.Word)) {
                occurrencesObj.words[b.Word] = {};
                occurrencesObj.count++;
            }
            
            // ensure OccurrenceHour exists as property on Word object
            if (occurrencesObj.words[b.Word].hasOwnProperty(b.OccurrenceHour)) {
                occurrencesObj.words[b.Word][b.OccurrenceHour] += b.Occurrences;
            }
            else {
                occurrencesObj.words[b.Word][b.OccurrenceHour] = b.Occurrences;
            }
        });

        return occurrencesObj;
    }
}
