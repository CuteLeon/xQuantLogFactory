$('#document').ready(() => {
    let lastContainer = undefined;
    $('.report-container').hide();

    $('.nav-link').click(e => {
        let target = $(e.target).data('target');
        console.log(target);

        let currentContainer = $(`#container_${target}`);
        if (lastContainer !== undefined) {
            lastContainer.hide();
        }

        if (currentContainer !== undefined &&
            currentContainer !== lastContainer) {
            currentContainer.show();
            lastContainer = currentContainer;
        }
    });
});