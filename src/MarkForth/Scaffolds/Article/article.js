window.addEventListener("load", () => {

    // Derive the title from the file name.
    const fileName = decodeURI(location.pathname.substring(location.pathname.lastIndexOf("/") + 1));
    document.title = fileName.substring(0, fileName.lastIndexOf("."));

    // Add a top-level heading to the document.
    const h1 = document.createElement("h1");
    h1.innerText = document.title;
    document.body.prepend(h1);

    // Number the sections of the document.
    function numberSections(parentPrefix, parent) {
        let counter = 1;
        for (const section of parent.querySelectorAll(":scope > section")) {
            const h1 = section.querySelector(":scope > h1");
            if (h1) {
                const prefix = (parentPrefix ?? "") + counter + ".";
                h1.innerHTML = prefix + " " + h1.innerHTML.trim();
                numberSections(prefix, section);
            }
            counter++;
        }
    }
    numberSections(null, document.body);
});
