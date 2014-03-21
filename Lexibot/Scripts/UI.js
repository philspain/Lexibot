/// <reference path='jquery-1.5.1.min.js' />

function UI() {
    this.topOfLastPost;
    this.scrollPostsOn = false;
    this.scrollOccurrencesOn = false;
    this.contentMoving = false;
    this.currentPosition = 1;
    this.elementHeights = new Array();
    this.heightIndex = 0;

    this.tryScrollPosts = function () {
        this.elementHeights = new Array();

        $('#first-posts').children().each(function () {
            var elem = $(this);
            ui.elementHeights.push(elem.height());
        });

        if ($('#posts').height() > $('#content-container').height() && !this.scrollPostsOn) {
            this.scrollPostsOn = true;
            this.scrollPosts();
        }
    }

    this.tryScrollOccurrences = function () {
        this.elementHeights = new Array();

        $('#first-occurrences').children().each(function () {
            var elem = $(this);
            ui.elementHeights.push(elem.height());
        });

        if ($('#occurrences').height() > $('#content-container').height() && !this.scrollOccurrencesOn) {
            this.scrollOccurrencesOn = true;
            this.scrollOccurrences();
        }
    }

    this.scrollPosts = function () {
        var processor = setInterval(function () {
            if (ui.autoScrollOn) {
                console.log(ui.heightIndex);
                $('#posts').children().each(function () {
                    $(this).animate({ 'top': '-=' + ui.elementHeights[ui.heightIndex] }, '800', 'linear', function () {

                        if ((parseInt($('#first-posts').css('top'), 10) == -$('#first').height()) &&
                        (parseInt($('#last-posts').css('top'), 10) == 0)) {
                            console.log('hello there');
                            var topHeight = $('#first-posts').height()
                            var elem = $('#first-posts').clone();
                            $('#first-posts').remove();

                            $('#last-posts').removeAttr('id');

                            $(elem).css('top', topHeight + 'px').attr('id', 'last-posts').appendTo('#posts');

                            $($('#posts div:first-child')).attr('id', 'first-posts');
                        }
                    });
                });

                if (ui.heightIndex < ui.elementHeights.length - 1) {
                    ui.heightIndex++;
                }
                else {
                    ui.heightIndex = 0;
                }
            }
            else {
                clearInterval(processor);
            }
        }, 4000);
    };

    this.scrollOccurrences = function () {
        var processor = setInterval(function () {
            if (ui.scrollOccurrencesOn) {
                console.log(ui.heightIndex);
                $('#occurrences').children().each(function () {
                    $(this).animate({ 'top': '-=' + ui.elementHeights[ui.heightIndex] }, '800', 'linear', function () {

                        if ((parseInt($('#first-occurrences').css('top'), 10) == -$('#first-occurrences').height()) &&
                        (parseInt($('#last-occurrences').css('top'), 10) == 0)) {
                            console.log('hello there');
                            var topHeight = $('#first-occurrences').height()
                            var elem = $('#first-occurrences').clone();
                            $('#first-occurrences').remove();

                            $('#last-occurrences').removeAttr('id');

                            $(elem).css('top', topHeight + 'px').attr('id', 'last-occurrences').appendTo('#occurrences');

                            $($('#occurrences div:first-child')).attr('id', 'first-occurrences');
                        }
                    });
                });

                if (ui.heightIndex < ui.elementHeights.length - 1) {
                    ui.heightIndex++;
                }
                else {
                    ui.heightIndex = 0;
                }
            }
            else {
                clearInterval(processor);
            }
        }, 4000);
    };

    this.moveContent = function (toPosition) {
        var currPosition = parseInt($('#content-container').css('left'), 10);
        var positionModifier;
        var distanceToMove;


        if (toPosition == 0 && !this.contentMoving && currPosition != 0) {
            this.contentMoving = true;
            distanceToMove = Math.abs(parseInt($('#content-container').css('left'), 10));
            positionModifier = '+=';
        }

        if (toPosition == 1 && !this.contentMoving && currPosition != -$(window).width()) {
            

            if (currPosition != -($(window).width())) {
                this.contentMoving = true;
                if (currPosition == 0) {
                    positionModifier = '-=';
                }
                else if (currPosition == -($(window).width() * 2)) {
                    positionModifier = '+=';
                }

                distanceToMove = $(window).width();
            }
        }

        var rightModifier = $('#content-container').width() - $('#right').width();

        if (toPosition == 2 && !this.contentMoving && Math.abs(currPosition) != rightModifier) {
            this.contentMoving = true;
            distanceToMove = rightModifier - Math.abs(parseInt($('#content-container').css('left'), 10));
            positionModifier = '-=';
        }

        $('#content-container').animate(
            { 'left': positionModifier + distanceToMove },
            '800',
            'linear',
            function () { ui.contentMoving = false; }
        );
    };

    this.navigate = function (direction) {
        var destinationIndex;

        switch (direction) {
            case 0:
                destinationIndex = 0;
                break;
            case 1:
                destinationIndex = 1;
                break;
            case 2:
                destinationIndex = 2;
                break;
            case 'left-nav':
                destinationIndex = this.currentPosition - 1;
                break;
            case 'right-nav':
                destinationIndex = this.currentPosition + 1;
                break;
        }

        if (destinationIndex != this.currentPosition &&
            destinationIndex !== null) {

            if (destinationIndex < this.currentPosition) {
                if (destinationIndex == 0) {
                    $('#left-nav').css({ 'visibility': 'hidden' });
                }

                if ($('#right-nav').css('visibility') == 'hidden') {
                    $('#right-nav').css({ 'visibility': 'visible' });
                }
            }

            if (destinationIndex > this.currentPosition) {

               
                if (destinationIndex == 2) {
                    $('#right-nav').css({ 'visibility': 'hidden' });
                }

                if ($('#left-nav').css('visibility') == 'hidden') {
                    $('#left-nav').css({ 'visibility': 'visible' });
                }
            }

            this.currentPosition = destinationIndex;
            this.moveContent(destinationIndex);
        }
    }

    this.resizeContentElements = function () {
        //resize content-container
        var contentHeight = $(window).height() - $('#header-container').outerHeight();

        $('#content-container').css({ 
            'width': ($(window).width() * 3) + 'px',
            'left': '-' + $(window).width() + 'px',
            'top': $('#header-container').outerHeight() + 'px'
        });

        //resize content panes
        $('.pane').height(contentHeight);
        $('.pane').width($(window).width());

        //set individual content pane positions
        $('#left').css({
            'left': '0px',
            'top': '0px'
        });

        $('#middle').css({
            'left': $(window).width() + 'px',
            'top': '0px'
        });

        $('#right').css({
            'right': '0px',
            'top': '0px'
        });

        var postWidth = Math.floor(($(window).width() - ($(".nav-container").width() * 2)) * 0.9);
        var postHeight = Math.floor(contentHeight * 0.9);
        var postMargin = Math.floor((contentHeight * 0.1) / 2);

        //resize nav elements used for moving content container
        $('.nav-direction').css({ 
            'height': contentHeight + 'px',
            'top': $('#header-container').outerHeight() + 'px'
        });

        $('#left-nav').css({
            'left': '0px'
        });

        var leftWidth = $(window).width() - postMargin;
        $('#right-nav').css({
            'right': '0px'
        });

        var navHeight = $('#left-nav').innerHeight();
        var imgMargin = Math.floor((navHeight - $('#left-nav img').height()) / 2);
        $('.nav-direction img').css('margin-top', imgMargin + 'px');

        $('#post-container').css({
            'margin': postMargin + 'px auto 0px auto',
            'height': postHeight + 'px',
            'width': postWidth + 'px'
        });

        $('#posts').width($('#post-container').width() - $('#scroll-menu-container').outerWidth());

        $('#occurrence-container').css({
            'margin': postMargin + 'px auto 0px auto',
            'height': postHeight + 'px',
            'width': postWidth + 'px'
        });

        //vertical align loader
        var loaderMargin = (($('#occurrence-container').height() - $('#occurrence-loader img').height()) / 2) + 'px';
        $('#occurrence-loader img').css('margin-top', loaderMargin);

        $('#occurrences').width($('#occurrence-container').width() - $('#occurrence-menu-container').outerWidth());
    };

    this.loadGraph = function (graphType) {
        switch (graphType) {
            case 'HeatMap':
                this.loadHeatMap();
                break;
            default:
                break;
        }
    };

    this.loadHeatMap = function () {
        setTimeout(function () {
            var occurrencesObj = requests.retrieveOccurrences();
            var tableHTML = '';

            $.each(Object.keys(occurrencesObj.words), function (a, b) {
                tableHTML += '<tr><th>' + b + '</th>';
                
                for (var i = 0; i <= 23; i++) {
                    tableHTML += '<td>';
                    
                    if (occurrencesObj.words[b].hasOwnProperty(i)) {
                        tableHTML += occurrencesObj.words[b][i];
                    }
                    else {
                        tableHTML += '0';
                    }

                    tableHTML += '</td>';
                }
            });

            $('#table1 tbody').append(tableHTML);

            $('#table1 td').graphup({
                painter: 'bubbles',
                bubblesDiameter: 80, // px
                callBeforePaint: function () {
                    // Hide all values under 50%
                    if (this.data('percent') < 50) {
                        this.text('');
                    }
                }
            });

            ui.showGraph();
        }, 500);
    }

    this.showGraph = function () {
        $('#occurrence-loader').css('visibility', 'hidden');
        $('#occurrences').css('visibility', 'visible');
    }
};