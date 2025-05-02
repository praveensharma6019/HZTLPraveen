import { TextField, RichTextField, RichText } from '@sitecore-jss/sitecore-jss-nextjs';

type FieldsAccordionListFieldsFields = {
  title?: TextField;
  description?: RichTextField;
};

type FieldsAccordionListFields = {
  id?: string;
  url?: string;
  name?: string;
  displayName?: string;
  fields?: FieldsAccordionListFieldsFields;
};

type Fields = {
  accordionList?: FieldsAccordionListFields[];
};

type AccordionProps = {
  fields?: Fields;
};

const AccordionData = (props: AccordionProps): JSX.Element => (
  <div className="accordion" id="accordionExample">
    {props.fields?.accordionList?.map((item, index) => {
      const collapseId = `collapse${index}`; // Generate unique ID for each item

      return (
        <div className="accordion-item" key={collapseId}>
          <h2 className="accordion-header">
            <button
              className="accordion-button"
              type="button"
              data-bs-toggle="collapse"
              data-bs-target={`#${collapseId}`}
              aria-expanded="false"
              aria-controls={collapseId}
            >
              {item.fields?.title?.value}
            </button>
          </h2>
          <div
            id={collapseId}
            className="accordion-collapse collapse"
            data-bs-parent="#accordionExample"
          >
            <div className="accordion-body">
              <RichText field={item.fields?.description}></RichText>
            </div>
          </div>
        </div>
      );
    })}
  </div>
);

export default AccordionData;
