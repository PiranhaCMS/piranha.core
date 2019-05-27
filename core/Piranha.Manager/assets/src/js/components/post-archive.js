Vue.component("post-archive", {
    props: ["uid", "id"],
    methods: {
    },
    mounted: function () {
    },
    beforeDestroy: function () {
    },
    template:
        "<div :id='uid'>" +
        "  <div class='form-inline mb-2'>" +
        "      <button class='btn btn-primary mr-1' href='#'>All</button>" +
        "      <button class='btn btn-light mr-1' href='#'>Drafts</button>" +
        "      <button class='btn btn-light mr-1' href='#'>Scheduled</button>" +
        "      <select class='form-control mr-1'>" +
        "        <option>All types</option>" +
        "      </select>" +
        "      <select class='form-control'>" +
        "        <option>All categories</option>" +
        "      </select>" +
        "      <button class='btn btn-primary btn-labeled float-right'><i class='fas fa-plus'></i>Add item</button>" +
        "  </div>" +
        "  <div class='mb-4'>" +
        "    <input type='text' class='form-control' placeholder='Search for a post in the archive'>" +
        "  </div>" +
        "  <table class='table'>" +
        "    <tbody>" +
        "      <tr>" +
        "        <td>" +
        "          <a href='#'>Techniques For Securing Pages</a> <small class='text-muted'>| Published: 2019-03-22</small>" +
        "        </td>" +
        "        <td>" +
        "          Blog post" +
        "        </td>" +
        "        <td>" +
        "          Piranha CMS" +
        "        </td>" +
        "        <td class='actions one'>" +
        "          <a href='#' class='danger'><i class='fas fa-trash'></i></a>" +
        "        </td>" +
        "      </tr>" +
        "      <tr>" +
        "        <td>" +
        "          <a href='#'>A Sneak Preview Of Blocks</a> <small class='text-muted'>| Published: 2018-05-02</small>" +
        "        </td>" +
        "        <td>" +
        "          Blog post" +
        "        </td>" +
        "        <td>" +
        "          Templates" +
        "        </td>" +
        "        <td class='actions one'>" +
        "          <a href='#' class='danger'><i class='fas fa-trash'></i></a>" +
        "        </td>" +
        "      </tr>" +
        "    </tbody>" +
        "  </table>" +
        "  <nav aria-label='...'>" +
        "    <ul class='pagination'>" +
        "      <li class='page-item disabled'>" +
        "        <a class='page-link' href='#' tabindex='-1' aria-disabled='true'>Previous</a>" +
        "      </li>" +
        "      <li class='page-item'><a class='page-link' href='#'>1</a></li>" +
        "      <li class='page-item active' aria-current='page'>" +
        "        <a class='page-link' href='#'>2 <span class='sr-only'>(current)</span></a>" +
        "      </li>" +
        "      <li class='page-item'><a class='page-link' href='#'>3</a></li>" +
        "      <li class='page-item'>" +
        "        <a class='page-link' href='#'>Next</a>" +
        "      </li>" +
        "    </ul>" +
        "  </nav>" +
        "</div>"
});
