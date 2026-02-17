let allEntries = [];
let calendar;

function loadTagsAndFilters() {
    $.get('/api/tag', function (tags) {
        var $formContainer = $('#tags');
        var $filterContainer = $('#tagFilters');

        $formContainer.empty();
        $filterContainer.find('.filter-tag:not([data-tag="all"])').remove();

        tags.forEach(tag => {
            const tagName = tag.customName || tag.resourceKey;
            const checkboxHtml = `
                <div class="form-check me-3">
                    <input class="form-check-input tag-checkbox" type="checkbox" value="${tag.id}" id="tag_${tag.id}">
                    <label class="form-check-label" for="tag_${tag.id}">${tagName}</label>
                </div>`;
            $formContainer.append(checkboxHtml);

            const filterBtnHtml = `
                <button class="btn btn-sm btn-outline-lumo filter-tag" data-tag="${tagName}">
                    ${tagName}
                </button>`;
            $filterContainer.append(filterBtnHtml);
        });
    });
}

function renderCalendarSchedules(filterTag = 'all') {
    calendar.clear();
    const filtered = (filterTag === 'all')
        ? allEntries
        : allEntries.filter(e => e.tags && e.tags.includes(filterTag));

    const schedules = filtered.map(e => {
        const moodColor = mapMoodToColor(e.moodRating);
        return {
            id: String(e.id),
            calendarId: '1',
            title: e.title,
            category: 'allday',
            start: e.entryDate,
            end: e.entryDate,
            isAllDay: true,
            bgColor: moodColor,
            borderColor: e.isFavorite ? '#ffbb00' : moodColor,
            color: '#111827',
            raw: {
                content: e.content,
                tags: e.tags,
                moodRating: e.moodRating,
                isFavorite: e.isFavorite
            }
        };
    });
    calendar.createSchedules(schedules);
}

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

$(document).ready(function () {
    loadTagsAndFilters();

    calendar = new tui.Calendar('#tui-calendar', {
        defaultView: 'month',
        taskView: false,
        scheduleView: ['time', 'allday', 'milestone'],
        useCreationPopup: false,
        useDetailPopup: false,
        isReadOnly: true,
        month: { startDayOfWeek: 1 }
    });

    function loadEntries() {
        $.get('/api/diary', function (entries) {
            allEntries = entries;
            renderCalendarSchedules('all');
        });
    }

    loadEntries();

    $(document).on('click', '.filter-tag', function () {
        $('.filter-tag').removeClass('active btn-lumo').addClass('btn-outline-lumo');
        $(this).addClass('active btn-lumo').removeClass('btn-outline-lumo');
        renderCalendarSchedules($(this).data('tag'));
    });

    $('#createEntryForm').on('submit', function (e) {
        e.preventDefault();
        const data = {
            title: $('#title').val(),
            content: $('#content').val(),
            entryDate: $('#entryDate').val(),
            moodRating: parseInt($('#moodRating').val()),
            isFavorite: $('#isFavorite').is(':checked'),
            tagIds: $('.tag-checkbox:checked').map(function () { return parseInt($(this).val()); }).get()
        };

        $.ajax({
            url: '/api/diary',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function () {
                $('#createEntryForm')[0].reset();
                $('#createEntryCard').slideUp();
                const btn = $('#toggleEntryForm');
                btn.text(btn.data('text-hide'));
                loadEntries();
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
    $('#prevMonth').click(function () { calendar.prev(); updateMonthLabel(); });
    $('#nextMonth').click(function () { calendar.next(); updateMonthLabel(); });
    $('#today').click(function () { calendar.today(); updateMonthLabel(); });

    calendar.on('clickSchedule', function (event) {
        const sch = event.schedule;
        $('#entryModalTitle').text(sch.title);
        $('#entryModalContent').text(sch.raw.content);
        $('#entryModalTags').html(sch.raw.tags?.map(t => `<span class="badge bg-secondary me-1">${t}</span>`).join('') || '');
        $('#entryModalFavorite').html(sch.raw.isFavorite ? '<span class="text-warning">⭐ Favorite</span>' : '<span class="text-muted">Not Favorite</span>');

        var myModal = new bootstrap.Modal(document.getElementById('entryModal'));
        myModal.show();

        $('#editEntryBtn').off('click').on('click', function () {
            window.location.href = '/Diary/Edit/' + sch.id;
        });

        $('#deleteEntryBtn').off('click').on('click', function () {
            const confirmMsg = $('#tui-calendar').data('confirm-delete');
            if (confirm(confirmMsg)) {
                $.ajax({
                    url: '/api/diary/' + sch.id,
                    type: 'DELETE',
                    success: function () {
                        myModal.hide();
                        loadEntries();
                    }
                });
            }
        });
    });

    $('#toggleEntryForm').click(function () {
        const btn = $(this);
        const addText = btn.data('text-add');
        const hideText = btn.data('text-hide');

        $('#createEntryCard').slideToggle();
        btn.text(btn.text() === addText ? hideText : addText);
    });
});