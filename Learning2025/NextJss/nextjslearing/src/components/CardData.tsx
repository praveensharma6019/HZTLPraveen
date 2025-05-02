import {
  TextField,
  RichTextField,
  LinkField,
  ImageField,
  RichText,
} from '@sitecore-jss/sitecore-jss-nextjs';

type FieldsCardDetailsFieldsFields = {
  link?: LinkField;
  image?: ImageField;
  title?: TextField;
  description?: RichTextField;
};

type FieldsCardDetailsFields = {
  id?: string;
  url?: string;
  name?: string;
  displayName?: string;
  fields?: FieldsCardDetailsFieldsFields;
};

type Fields = {
  CardDetails?: FieldsCardDetailsFields[];
  title?: TextField;
};

type CardDataProps = {
  fields?: Fields;
};
const baseurl = 'https://scxp104sc.dev.local/';

const CardData = (props: CardDataProps): JSX.Element => (
  <div className="CardSection">
    {/* Displaying the title if it exists */}
    {props.fields?.title?.value && <h2>{props.fields?.title?.value}</h2>}
    <div className="row CardSectionDetails">
      {props.fields?.CardDetails?.map((item) => {
        return (
          // eslint-disable-next-line react/jsx-key
          <div className="col-sm-3">
            <div className="card" style={{ width: '18rem' }}>
              <img
                className="card-img-top"
                src={baseurl + item.fields?.image?.value?.src}
                alt="Card image cap"
              />
              <div className="card-body">
                <h5 className="card-title">{item.fields?.title?.value}</h5>
                <RichText className="card-text" field={item.fields?.description}></RichText>
                <a href={item.fields?.link?.value.href} className="btn btn-primary">
                  {item.fields?.link?.value.text}
                </a>
              </div>
            </div>
          </div>
        );
      })}
    </div>
  </div>
);

export default CardData;
