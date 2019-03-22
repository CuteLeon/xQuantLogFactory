$('#document').ready(() => {
    $('.nav-link').click(e => {
        let target = $(e.target).data('target');
        console.log(target);
    });
});