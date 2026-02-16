// Toggle form visibility
$('#toggleEntryForm').click(function () {
    $('#createEntryCard').slideToggle(); // slide animation for showing/hiding
    $(this).text(function (i, text) {
        return text === "Add New Entry" ? "Hide Form" : "Add New Entry";
    });
});
function loadTags() {
    $.get('/api/tag', function (tags) {
        var $container = $('#tags');
        $container.empty();
        tags.forEach(tag => {
            const checkboxHtml = `
                <div class="form-check me-3">
                    <input class="form-check-input tag-checkbox" type="checkbox" value="${tag.id}" id="tag_${tag.id}">
                    <label class="form-check-label" for="tag_${tag.id}">${tag.customName}</label>
                </div>
            `;
            $container.append(checkboxHtml);
        });
    });
}

$(document).ready(function () {
    loadTags();

    var calendar = new tui.Calendar('#tui-calendar', {
        defaultView: 'month',
        taskView: false,
        scheduleView: ['time', 'allday', 'milestone'],
        useCreationPopup: true,
        useDetailPopup: false,
        isReadOnly: true,
        month: { startDayOfWeek: 1 }
    });

    function mapMoodToColor(mood) {
        switch (mood) {
            case 1: return '#fc6969';
            case 2: return '#ff8533';
            case 3: return '#fcdf69';
            case 4: return '#30e889';
            case 5: return '#33ffd6';
            default: return '#fef3c7';
        }
    }
    function isFavoriteBorder(isFavorite) {
        switch (isFavorite) {
            case true: return '#ffbb00';
            case false: return '#000000';
            default: return '#000000';
        }
    }

    function loadEntries() {
        $.get('/api/diary', function (entries) {
            calendar.clear();
            const schedules = entries.map(e => {
                return {
                    id: String(e.id),
                    calendarId: '1',
                    title: e.title,
                    category: 'allday',
                    start: e.entryDate,
                    end: e.entryDate,
                    isAllDay: true,
                    bgColor: mapMoodToColor(e.moodRating),

                    borderColor: isFavoriteBorder(e.isFavorite),

                    color: '#111827',
                    raw: { content: e.content, tags: e.tags, moodRating: e.moodRating, isFavorite: e.isFavorite }
                };
            });
            calendar.createSchedules(schedules);
        });
    }

    loadEntries();

    $('#createEntryForm').on('submit', function (e) {
        e.preventDefault();
        const data = {
            title: $('#title').val(),
            content: $('#content').val(),
            entryDate: $('#entryDate').val(),
            moodRating: parseInt($('#moodRating').val()),
            isFavorite: $('#isFavorite').is(':checked'),
            tagIds: $('.tag-checkbox:checked').map(function () { return $(this).val(); }).get()
        };

        $.ajax({
            url: '/api/diary',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (entry) {
                calendar.createSchedules([{
                    id: String(entry.id),
                    calendarId: '1',
                    title: entry.title,
                    category: 'allday',
                    start: entry.entryDate,
                    end: entry.entryDate,
                    isAllDay: true,
                    bgColor: mapMoodToColor(entry.moodRating),

                    borderColor: isFavoriteBorder(entry.isFavorite),

                    color: '#111827',
                    raw:
                    {
                        content: entry.content,
                        tags: entry.tags,
                        moodRantig: entry.moodRating,
                        isFavorite: entry.isFavorite
                    }
                }]);
                $('#createEntryForm')[0].reset();
                loadTags();
            }
        });
    });

    function updateMonthLabel() {
        const current = calendar.getDate();
        const month = current.getMonth() + 1;
        const year = current.getFullYear();
        document.getElementById('currentMonth').innerText = `${year} / ${month}`;
    }

    updateMonthLabel();
    document.getElementById('prevMonth').addEventListener('click', function () { calendar.prev(); updateMonthLabel(); });
    document.getElementById('nextMonth').addEventListener('click', function () { calendar.next(); updateMonthLabel(); });
    document.getElementById('today').addEventListener('click', function () { calendar.today(); updateMonthLabel(); });

    calendar.on('clickSchedule', function (event) {
        const sch = event.schedule;
        $('#entryModalTitle').text(sch.title);
        $('#entryModalContent').text(sch.raw.content);
        $('#entryModalTags').html(
            sch.raw.tags?.map(t => `<span class="badge bg-secondary me-1">${t}</span>`).join('') || ''
        );
        $('#entryModalFavorite').html(
            sch.raw.isFavorite
                ? '<span class="text-warning">⭐ Favorite</span>'
                : '<span class="text-muted">Not Favorite</span>'
        );
        var myModal = new bootstrap.Modal(document.getElementById('entryModal'));
        myModal.show();

        $('#editEntryBtn').off('click').on('click', function () {
            // sch.id to ID wpisu z kalendarza
            window.location.href = '/Diary/Edit/' + sch.id;
        });

        $('#deleteEntryBtn').off('click').on('click', function () {
            if (confirm('Are you sure you want to delete this entry?')) {
                $.ajax({
                    url: '/api/diary/' + sch.id,
                    type: 'DELETE',
                    success: function () {
                        calendar.deleteSchedule(sch.id, sch.calendarId);
                        myModal.hide();
                    }
                });
            }
        });
    });
});