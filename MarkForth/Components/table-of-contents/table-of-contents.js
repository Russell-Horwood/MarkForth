window.addEventListener("load", () => {
    customElements.define('table-of-contents',
        class extends HTMLElement {
            constructor() {
                super();

                const tableElement = document.getElementById('table-of-contents');
                const div = tableElement.content.querySelector("div");

                const ol = document.createElement("ol");
                for (const entry of this.getEntries(document.body)) {
                    ol.appendChild(this.getLi(entry));
                }
                div.appendChild(ol);

                this.attachShadow({ mode: 'open' })
                    .appendChild(tableElement.content.cloneNode(true));
            }

            *getEntries(scope) {
                for (const section of scope.querySelectorAll(":scope > section")) {
                    const h1 = section.querySelector(":scope > h1");
                    if (h1) {
                        const text = h1.innerHTML.trim().replace(/(\r\n|\n|\r)/gm, "");
                        h1.id = text.toLowerCase();
                        yield {
                            "children": this.getEntries(section),
                            "id": h1.id,
                            "text": text
                        };
                    }
                }
            }

            getLi(entry) {
                const a = document.createElement("a");
                a.href = "#" + entry.id;
                a.innerHTML = entry.text;

                const ol = document.createElement("ol");
                for (const child of entry.children) {
                    ol.appendChild(this.getLi(child));
                }

                const li = document.createElement("li");
                li.appendChild(a);
                li.appendChild(ol);

                return li;
            }
        }
    );
});
