window.addEventListener("load", () => {
    customElements.define('code-block',
        class extends HTMLElement {

            copyButton;
            span;

            constructor() {
                super();

                const codeBlock = document.getElementById('code-block').content.cloneNode(true);

                this.span = this.querySelector("span[slot='code']");
                this.span.innerHTML = this.span.innerHTML.trim();

                // 'pre' tags may have been added to code in source documents
                // to prevent editor auto-formatters from reformatting source code.
                if (this.span.innerHTML.startsWith("<pre"))
                    this.span.innerHTML = this.span.children[0].innerHTML;

                this.copyButton = codeBlock.querySelector("svg");
                this.copyButton.addEventListener("click", this.onCopyClick.bind(this));

                this.attachShadow({ mode: 'open' }).appendChild(codeBlock);
            }

            onCopyClick() {
                if (this.span) {
                    navigator.clipboard.writeText(
                        this.span.innerHTML.trim()
                    );
                }

                this.copyButton.classList.toggle("active");
                setTimeout(
                    () => this.copyButton.classList.toggle('active'),
                    1000
                );
            }
        }
    );
});
